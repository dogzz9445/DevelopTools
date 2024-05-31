﻿using DDT.Core.WidgetSystems.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Bases;

public class WidgetGeneratorAttribute : Attribute
{
    public WidgetGenerator WidgetGenerator { get; private set; }

    public WidgetGeneratorAttribute(string name,
        string description,
        string menuPath,
        int menuOrder,
        CreateWidgetViewModelBase<WidgetViewModelBase> createWidget)
    {
        WidgetGenerator = new WidgetGenerator(
            name: name,
            description: description,
            menuPath: menuPath,
            menuOrder: menuOrder,
            createWidget: createWidget
        );
    }
}
