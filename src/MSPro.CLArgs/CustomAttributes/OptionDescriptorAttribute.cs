﻿using System;



namespace MSPro.CLArgs
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OptionDescriptorAttribute : Attribute
    {
        public OptionDescriptorAttribute(string optionName, string tag = null)
        {
            this.OptionName = optionName;
            this.Tags       = new[] {tag ?? optionName};
            this.Required   = false;
        }



        public string OptionName { get; set; }
        public string[] Tags { get; set; }
        public string Description { get; set; }
        public object Default { get; set; }
        public bool Required { get; set; }



        public new string ToString() =>
            $"{this.OptionName}: [{string.Join(",", this.Tags)}], required={this.Required}, Default={this.Default}";
    }
}