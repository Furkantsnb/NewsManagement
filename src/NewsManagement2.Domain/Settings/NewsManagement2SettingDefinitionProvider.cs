using Volo.Abp.Settings;

namespace NewsManagement2.Settings;

public class NewsManagement2SettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(NewsManagement2Settings.MySetting1));
    }
}
