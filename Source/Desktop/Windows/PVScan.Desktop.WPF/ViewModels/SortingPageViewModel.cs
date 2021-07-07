using PVScan.Core.Models;
using PVScan.Desktop.WPF.ViewModels.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class SortingPageViewModel : BaseViewModel
    {
        public SortingPageViewModel()
        {
            InitSortingFields();

            InitDefaultSorting();
            SetStateToCurrentSorting();

            ToggleOrderCommand = new Command(() =>
            {
                Descending = !Descending;

                ToggleApplySortingEnabled();
            });

            ApplySortingCommand = new Command(() =>
            {
                if (AppliedAndCurrentSortingSame())
                {
                    // Do nothing..
                    return;
                }

                CurrentSorting = new Sorting()
                {
                    Field = SelectedSortingField,
                    Descending = Descending,
                };

                ToggleApplySortingEnabled();

                // Send message that sorting has been applied
                MessagingCenter.Send(this, nameof(SortingAppliedMessage), new SortingAppliedMessage()
                {
                    NewSorting = CurrentSorting,
                });
            });

            ResetSortingCommand = new Command(() =>
            {
                // Reset sorting to default
                var defaultSorting = Sorting.Default();

                Descending = defaultSorting.Descending;
                SelectedSortingField = defaultSorting.Field;

                ToggleApplySortingEnabled();
            });

            SortingFieldSelectedCommand = new Command((object newField) =>
            {
                if ((SortingField)newField != SelectedSortingField)
                {
                    SelectedSortingField = (SortingField)newField;
                }

                ToggleApplySortingEnabled();
            });
        }

        private void InitSortingFields()
        {
            AvailableSortingFields = new ObservableCollection<SortingField>();

            foreach (var v in Enum.GetValues(typeof(SortingField)))
            {
                var field = (SortingField)v;

                if (field != SortingField.None)
                {
                    AvailableSortingFields.Add(field);
                }
            }
        }

        private void ToggleApplySortingEnabled()
        {
            if (AppliedAndCurrentSortingSame())
            {
                ApplySortingEnabled = false;
                return;
            }

            ApplySortingEnabled = true;
        }

        private bool AppliedAndCurrentSortingSame()
        {
            return SelectedSortingField == CurrentSorting.Field &&
                    Descending == CurrentSorting.Descending;
        }

        private void InitDefaultSorting()
        {
            CurrentSorting = Sorting.Default();
        }

        public void SetStateToCurrentSorting()
        {
            Descending = CurrentSorting.Descending;
            SelectedSortingField = CurrentSorting.Field;

            ToggleApplySortingEnabled();
        }

        public Sorting CurrentSorting { get; set; }

        public ObservableCollection<SortingField> AvailableSortingFields { get; set; }
        public SortingField SelectedSortingField { get; set; }
        public bool Descending { get; set; }

        // Toggle between ascending and descending order
        public ICommand ToggleOrderCommand { get; set; }

        public ICommand ApplySortingCommand { get; set; }
        public bool ApplySortingEnabled { get; set; }

        public ICommand ResetSortingCommand { get; set; }

        public ICommand SortingFieldSelectedCommand { get; set; }
    }
}
