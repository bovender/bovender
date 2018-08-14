﻿/* EnumBooleanConverter.cs
 * part of Daniel's XL Toolbox NG
 * 
 * Copyright 2014-2018 Daniel Kraus
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Bovender.Mvvm.Converters
{
    /// <summary>
    /// Converts enum values to booleans to enable easy use
    /// of enumerations with WPF radio buttons.
    /// </summary>
    /// <remarks>
    /// Credits to Scott @ http://stackoverflow.com/a/2908885/270712
    /// </remarks>
    /// <example><code><![CDATA[
    ///     <StackPanel>
    ///         <StackPanel.Resources>          
    ///             <local:EnumToBooleanConverter x:Key="ebc" />          
    ///         </StackPanel.Resources>
    ///         <RadioButton IsChecked="{Binding Path=YourEnumProperty,
    ///                      Converter={StaticResource ebc},
    ///                      ConverterParameter={x:Static local:YourEnumType.Enum1}}" />
    ///         <RadioButton IsChecked="{Binding Path=YourEnumProperty,
    ///                      Converter={StaticResource ebc},
    ///                      ConverterParameter={x:Static local:YourEnumType.Enum2}}" />
    ///     </StackPanel>    /// <Grid>
    /// ]]></code></example>
    public class EnumBooleanConverter : IValueConverter
    {
        #region IValueConverter interface

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }

        #endregion
    }
}
