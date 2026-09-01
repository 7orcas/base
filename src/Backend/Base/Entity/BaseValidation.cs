using System.Net.Mail;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Entity
{
    public abstract class BaseValidation <T> where T : _BaseDto
    {

        protected OrgEnt org;
        protected Dictionary<string, string> labels;
        protected List<Info> validations;
        protected List<ValidationMessageDto> messages;

        protected string mandatory;
        protected string maxLengthEx;
        protected string maxValueEx;
        protected string minValueEx;

        public BaseValidation (OrgEnt? org,  Dictionary<string, string>? labels)
        {
            this.org = org;
            this.labels = labels;

            mandatory = GetLabel("InvF", "Field can't be empty");
            maxLengthEx = GetLabel("InvL", "Field exceeds maximum length"); //has (%%) appended
            maxValueEx = GetLabel("MaxV", "Maximum value exceeded"); //has (%%) appended
            minValueEx = GetLabel("MinV", "Minimum value exceeded"); //has (%%) appended

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

        public ValidationDto? Validate(T dto)
        {
            messages = null;

            if (validations == null)
                throw new Exception("Validations are null");

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

            if (messages != null)
            {
                return new ValidationDto { 
                    Id = dto.Id,
                    Messages = messages,
                };
            }

            return null;
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
                    IsRequiredNew = info.IsRequiredNew
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
