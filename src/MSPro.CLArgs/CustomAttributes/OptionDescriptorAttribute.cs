﻿using System;
using JetBrains.Annotations;



namespace MSPro.CLArgs;

[AttributeUsage(AttributeTargets.Property)]
[PublicAPI]
public class OptionDescriptorAttribute : Attribute
{
    public OptionDescriptorAttribute(string optionName,
        string[] tags,
        bool required = false,
        object defaultValue = null,
        string helpText = null)
    {
        OptionName = optionName;
        Tags = tags;
        HelpText = helpText;
        Default = defaultValue;
        Required = required;
    }



    public OptionDescriptorAttribute(string optionName,
        string[] tags,
        object defaultValue,
        string helpText = null)
    {
        OptionName = optionName;
        Tags = tags;
        HelpText = helpText;
        Default = defaultValue;
    }



    public OptionDescriptorAttribute(string optionName,
        string tag = null,
        bool required = false,
        object defaultValue = null,
        string helpText = null)
        : this(optionName, tag != null
            ? new[] { tag }
            : null, required, defaultValue, helpText)
    {
    }



    public OptionDescriptorAttribute(char tag,
        string optionName,
        bool required = false,
        object defaultValue = null,
        string helpText = null)
        : this(optionName, new[] { tag.ToString() }, required, defaultValue, helpText)
    {
    }



    public string AllowMultiple { get; set; }
    public bool Required { get; set; }
    public object Default { get; set; }
    public string HelpText { get; set; }
    public string[] Tags { get; set; }
    public string OptionName { get; set; }
    public string AllowMultipleSplit { get; set; }
}