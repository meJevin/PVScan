using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels.Messages;
using PVScan.Mobile.ViewModels.Messages.Filtering;
using PVScan.Mobile.ViewModels.Messages.Scanning;
using PVScan.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;

namespace PVScan.Mobile.ViewModels
{
    // Please take these out into a separate service THIS DOESN'T BELONG HERE!!
    public enum SortingField
    {
        None,
        Date,
        Text,
        Format
    };

    public class Sorting
    {
        public SortingField Field { get; set; }
        public bool Descending { get; set; }

        public static Sorting Default() => new Sorting()
        {
            Field = SortingField.Date,
            Descending = true,
        };
    }

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
            });

            ApplySortingCommand = new Command(() =>
            {
                if (SelectedSortingField == CurrentSorting.Field &&
                    Descending == CurrentSorting.Descending)
                {
                    // Do nothing..
                    return;
                }

                CurrentSorting = new Sorting()
                {
                    Field = SelectedSortingField,
                    Descending = Descending,
                };

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
            });

            SortingFieldSelectedCommand = new Command((object newField) =>
            {
                if ((SortingField)newField != SelectedSortingField)
                {
                    SelectedSortingField = (SortingField)newField;
                }
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

        private void InitDefaultSorting()
        {
            CurrentSorting = Sorting.Default();
        }

        public void SetStateToCurrentSorting()
        {
            Descending = CurrentSorting.Descending;
            SelectedSortingField = CurrentSorting.Field;
        }

        public Sorting CurrentSorting { get; set; }

        public ObservableCollection<SortingField> AvailableSortingFields { get; set; }
        public SortingField SelectedSortingField { get; set; }
        public bool Descending { get; set; }

        // Toggle between ascending and descending order
        public ICommand ToggleOrderCommand { get; set; }

        public ICommand ApplySortingCommand { get; set; }
        public ICommand ResetSortingCommand { get; set; }

        public ICommand SortingFieldSelectedCommand { get; set; }

        //public event EventHandler SortingApplied;
    }
}
