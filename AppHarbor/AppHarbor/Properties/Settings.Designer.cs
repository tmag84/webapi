﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppHarbor.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=e873e7e5-cf79-46de-aa5e-a63600f04b8f.sqlserver.sequelizer.com;Initial" +
            " Catalog=dbe873e7e5cf7946deaa5ea63600f04b8f;User ID=kzqeetgsvkjctkog;Password=bt" +
            "Fsh6mwoAd52m7k5ovrvTQ6wFhEkGr4G38jbP6ZiagUfgprt6PjczwpLH5HXPkN;MultipleActiveRes" +
            "ultSets=True")]
        public string connString {
            get {
                return ((string)(this["connString"]));
            }
        }
    }
}