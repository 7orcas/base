using GC = Backend.GlobalConstants;

namespace Backend.Base.Config.Ent
{
    public class ConfigVal : BaseValidation <ConfigDto>
    {
        private ConfigServiceI _configService;

        public ConfigVal(SessionEnt session,
            ConfigServiceI ConfigService)
            : base(session) 
        {
            _configService = ConfigService;
        }

        protected override void Configure()
        {
            Add(nameof(ConfigDto.Label.LangCode), "LangCode")
                .resetIsRequired()
                .setMaxLength(4);

        }
    }
}
