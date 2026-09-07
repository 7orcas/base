using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Superpower.Model;
using System.Net.Mail;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Entity
{
    public abstract class BaseValidation <T> where T : _BaseDto
    {
        protected SessionEnt session;
        protected OrgEnt org;
        protected Dictionary<string, string> labels;
        protected List<Info> validations;
        protected List<ValidationMessageDto> messages;

        protected string mandatory;
        protected string maxLengthEx;
        protected string maxValueEx;
        protected string minValueEx;
        protected string verX;
        protected string verC;
        protected string noPerm;

        public BaseValidation (SessionEnt session)
        {
            this.session = session;
            this.org = session.Org;
            this.labels = session.Labels;

            mandatory = GetLabel("InvF", "Field can't be empty");
            maxLengthEx = GetLabel("InvL", "Field exceeds maximum length"); //has (%%) appended
            maxValueEx = GetLabel("MaxV", "Maximum value exceeded"); //has (%%) appended
            minValueEx = GetLabel("MinV", "Minimum value exceeded"); //has (%%) appended
            verX = GetLabel("VerX", "This record was changed on %% and is out of date. You must reload it."); //has (%%) appended
            verC = GetLabel("VerC", "Version Conflict");
            noPerm = GetLabel("PermNoC", "No Permission to change");

            Configure();
        }

        protected abstract void Configure();

        public Info Add(string name, string label)
        {
            if (validations == null)
                validations = new List<Info>();

            var info = new Info()
            {
                Name = name,
                Label = GetLabel(label, label)
            };
            validations.Add(info);
            return info;
        }

        public ValidationDto? Validate(T dto, VersionI? version)
        {
            return Validate(dto, null, version);
        }

        public ValidationDto? Validate(T dto, T? currentDto, VersionI? version)
        {
            if (validations == null)
                throw new Exception("Validations are null");

            messages = null;
            ValidateInternal(dto, currentDto, version);

            if (messages != null)
            {
                return new ValidationDto
                {
                    Id = dto.Id,
                    Messages = messages,
                };
            }

            return null;
        }

        private void ValidateInternal(T dto, T? currentDto, VersionI? version)
        {
            //Version has changed
            if (version != null && dto.Version != null && version.Version != dto.Version)
            {
                var zone = TimeZoneInfo.ConvertTime(version.Updated, session.TimeZone);
                var ex = verX.Replace(GC.LabelParameterPrefix, zone.ToString(GC.Date_TS_Format));
                AddMessage(verC, ex);
                return;
            }

            foreach (var info in validations)
            {
                var property = typeof(T).GetProperty(info.Name);

                if (property == null)
                    throw new Exception("Validation name is null:" + info.Name);

                var value = property.GetValue(dto);
                var valueS = null as string;
                int? valueI = null;

                if (property.PropertyType == typeof(string))
                    valueS = (string) value;
                if (property.PropertyType == typeof(int))
                    valueI = (int)value;


                //Required field
                if (info.IsRequired 
                    && (value == null
                    || (property.PropertyType == typeof(string) && string.IsNullOrEmpty (valueS))))
                    AddMessage(info.Label, mandatory);

                //Required new field
                if (info.IsRequiredNew
                    && dto.IsNew()
                    && (value == null
                    || (property.PropertyType == typeof(string) && string.IsNullOrEmpty(valueS))))
                    AddMessage(info.Label, mandatory);

                //Is service only
                if (info.IsServiceOnly
                    && !session.IsService
                    && value != null
                    && currentDto != null)
                {
                    var valueX = property.GetValue(currentDto);
                    if (valueX == null || !value.Equals(valueX))
                        AddMessage(info.Label, noPerm);
                }
                    

                //Max Length Exceeded
                if (info.MaxLength != null
                    && info.MaxLength > 0 
                    && value != null
                    && property.PropertyType == typeof(string)
                    && ((string)value).Length > info.MaxLength)
                {
                    var m = maxLengthEx.Replace(GC.LabelParameterPrefix, info.MaxLength.ToString());
                    AddMessage(info.Label, m);
                }

                //Max Value Exceeded
                if (info.MaxValue != null
                    && valueI != null
                    && valueI > info.MaxValue)
                {
                    var m = maxValueEx.Replace(GC.LabelParameterPrefix, info.MaxValue.ToString());
                    AddMessage(info.Label, m);
                }

                //Min Value Exceeded
                if (info.MinValue != null
                    && valueI != null
                    && valueI < info.MinValue)
                {
                    var m = minValueEx.Replace(GC.LabelParameterPrefix, info.MinValue.ToString());
                    AddMessage(info.Label, m);
                }

                //Call back
                info.Callback?.Invoke(dto);
            }
        }

        public DefinitionDto GetDefinition()
        {
            if (validations == null)
                throw new Exception("Validations are null");


            var dto = new DefinitionDto()
            {
                Fields = new List<FieldDto>()
            };

            foreach (var info in validations)
            {
                dto.Fields.Add(new FieldDto() {
                    Name = info.Name,
                    MaxLength = info.MaxLength,
                    MinValue = info.MinValue,
                    MaxValue = info.MaxValue,
                    IsRequired = info.IsRequired,
                    IsRequiredNew = info.IsRequiredNew,
                    IsServiceOnly = info.IsServiceOnly
                });
            }
            return dto;
        }



        protected ValidationMessageDto AddMessage(string label, string message)
        {
            return AddMessage(label, message, false);
        }

        protected ValidationMessageDto AddMessage(string label, string message, bool isWarning)
        {
            if (messages == null)
                messages = new List<ValidationMessageDto>();

            var m = new ValidationMessageDto
            {
                Message = label + ": " + message ,
                IsError = !isWarning,
                IsWarning = isWarning
            };
            messages.Add(m);
            return m;
        }


        protected string GetLabel(string langKey, string nullDefault)
        {
            if (labels != null && labels.ContainsKey(langKey))
                return labels[langKey];
            return nullDefault;
        }

        protected bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch (ArgumentException ex) 
            {
                return false;
            }
            catch (FormatException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public class Info
        {
            public string Name { get; set; }
            public string Label { get; set; }
            public int? MaxLength { get; set; } 
            public int? MinValue { get; set; }
            public int? MaxValue { get; set; }
            public bool IsRequired { get; set; } = true;
            public bool IsRequiredNew { get; set; } = false;
            public bool IsServiceOnly { get; set; } = false;
            public bool IsUniqueDb { get; set; } = false;
            public object[]? Values { get; set; }
            public Action<T> Callback { get; set; }


            public Info setMaxLength (int l)
            {
                MaxLength = l;
                return this;
            }

            public Info resetIsRequired()
            {
                IsRequired = false;
                return this;
            }

            public Info setIsServiceOnly()
            {
                IsServiceOnly = true;
                return this;
            }

            public Info setIsRequiredNew()
            {
                IsRequiredNew = true;
                return this;
            }

            public Info setUniqueDb()
            {
                IsUniqueDb = true;
                return this;
            }

            public Info setCallBack(Action<T> callback)
            {
                Callback = callback;
                return this;
            }
        }

    }
}
