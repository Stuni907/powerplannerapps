﻿using PowerPlannerUWP.Views;
using PowerPlannerAppDataLibrary.DataLayer;
using PowerPlannerAppDataLibrary.DataLayer.DataItems;
using PowerPlannerAppDataLibrary.DataLayer.DataItems.BaseItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ToolsPortable;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PowerPlannerAppDataLibrary.ViewModels.MainWindow.MainScreen.TasksOrEvents;
using PowerPlannerAppDataLibrary.ViewItems.BaseViewItems;
using PowerPlannerAppDataLibrary.Extensions;
using PowerPlannerAppDataLibrary;
using PowerPlannerAppDataLibrary.Helpers;
using PowerPlannerUWP.Controls;
using Windows.UI.Core;
using Windows.System;
using PowerPlannerUWP.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PowerPlannerUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddTaskOrEventView : PopupViewHostGeneric
    {
        public new AddTaskOrEventViewModel ViewModel
        {
            get { return base.ViewModel as AddTaskOrEventViewModel; }
            set { base.ViewModel = value; }
        }

        public AddTaskOrEventView()
        {
            this.InitializeComponent();

            TextBlockImageAttachments.Text = PowerPlannerResources.GetString("String_ImageAttachments");
        }

        private void UpdateHeaderText()
        {
            this.Title = GetHeaderText();
        }

        private string GetHeaderText()
        {
            switch (ViewModel.Type)
            {
                case PowerPlannerAppDataLibrary.ViewItems.TaskOrEventType.Event:
                    if (IsEditing())
                        return LocalizedResources.GetString("String_EditEvent").ToUpper();
                    else
                        return LocalizedResources.GetString("String_AddEvent").ToUpper();

                default:
                    if (IsEditing())
                        return LocalizedResources.GetString("String_EditTask").ToUpper();
                    else
                        return LocalizedResources.GetString("String_AddTask").ToUpper();
            }
        }

        private string GetStartTimeText()
        {
            switch (ViewModel.Type)
            {
                case PowerPlannerAppDataLibrary.ViewItems.TaskOrEventType.Event:
                    return LocalizedResources.GetString("String_StartTime");

                default:
                    return LocalizedResources.GetString("String_DueTime");
            }
        }

        private bool IsEditing()
        {
            return ViewModel.State == AddTaskOrEventViewModel.OperationState.Editing;
        }

        public override void OnViewModelSetOverride()
        {
            ViewModel.AutoAdjustEndTimes = false;

            StartTimePicker.Header = GetStartTimeText();
            EndTimePicker.Header = LocalizedResources.GetString("String_EndTime");
            ComboBoxTimeOptions.Header = LocalizedResources.GetString("String_Time");

            ViewModel.PropertyChanged += new WeakEventHandler<System.ComponentModel.PropertyChangedEventArgs>(ViewModel_PropertyChanged).Handler;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.Repeats):
                    // Ensure the recurrence control is loaded
                    FindName(nameof(RecurrenceControlContainer));
                    break;
            }
        }

        public override void OnViewModelLoadedOverride()
        {
            base.OnViewModelLoadedOverride();

            UpdateHeaderText();

            datePickerDate.Date = ViewModel.Date;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            // Clicking button doesn't take focus away from TextBasedTimePicker, so their edited value doesn't get committed...
            // Therefore we have to take focus away, and wait for the focus to actually switch
            if (ButtonSave.FocusState == FocusState.Unfocused)
            {
                RoutedEventHandler gotFocus = null;
                gotFocus = delegate
                {
                    ButtonSave.GotFocus -= gotFocus;
                    Save();
                };
                ButtonSave.GotFocus += gotFocus;
                ButtonSave.Focus(FocusState.Programmatic);
                return;
            }

            Save();
        }

        private void Save()
        {
            ViewModel.Save();
        }

        private bool _needsFocus = true;
        private void TbName_Loaded(object sender, RoutedEventArgs e)
        {
            if (_needsFocus == false)
                return;

            _needsFocus = false;

            if (IsEditing())
                return;

            tbName.Focus(FocusState.Programmatic);

            if (tbName.Text.Length > 0)
            {
                // Only when duplicating should this get hit
                tbName.SelectAll();
            }
        }

        private void DatePickerDate_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (datePickerDate.Date != null)
            {
                ViewModel.Date = datePickerDate.Date.Value.Date;
            }
        }

        private void TbName_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                Save();
            }
        }

        private void ButtonUpgradeToPremiumForRepeating_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.UpgradeToPremiumForRepeating();
        }

        private async void EditImages_RequestedAddImage(object sender, EventArgs e)
        {
            await ViewModel.AddNewImageAttachmentAsync();
        }

        private void Popup_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (KeyPressedHelpers.IsCtrlKeyPressed())
            {
                // Ctrl+Enter or Ctrl+S will save the popup
                if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.S)
                {
                    e.Handled = true;
                    Save();
                }
            }
        }
    }
}
