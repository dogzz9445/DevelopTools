﻿using DDT.Core.WidgetSystems.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wpf.Ui.Controls;

namespace DDT.Core.WidgetSystems.Bases;

[AttributeUsage(AttributeTargets.Class)]
public class WidgetGeneratorAttribute : Attribute
{
    public IServiceProvider Services { get; set; }
    public WidgetGenerator WidgetGenerator { get; private set; }

    public WidgetGeneratorAttribute(string name,
        string description,
        string menuPath,
        int menuOrder,
        Type targetType)
    {
        WidgetGenerator = new WidgetGenerator(
            name: name,
            description: description,
            menuPath: menuPath,
            menuOrder: menuOrder,
            targetType: targetType,
            createWidget: () => (WidgetViewModelBase)Activator.CreateInstance(targetType, Services)
        );
    }

    public void RegisterServices(IServiceProvider services)
    {
        Services = services;
        WidgetGenerator.CreateWidgetInternal = () =>
            (WidgetViewModelBase)Activator.CreateInstance(WidgetGenerator.TargetType, Services);
    }
}
