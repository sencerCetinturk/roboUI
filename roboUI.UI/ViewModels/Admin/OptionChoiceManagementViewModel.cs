using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using roboUI.Core.Interfaces;
using roboUI.Core.Models;
using roboUI.UI.Commands;

namespace roboUI.UI.ViewModels.Admin
{
   public class OptionChoiceManagementViewModel : BaseViewModel
    {
        private readonly IOptionChoiceService _optionChoiceService;
        private readonly IOptionGroupService _optionGroupService;

        private ObservableCollection<OptionChoice> _optionChoices;
        public ObservableCollection<OptionChoice> OptionChoices
        {
            get => _optionChoices;
            set => SetProperty(ref _optionChoices, value);
        }

        private OptionChoice? _selectedOptionChoice;
        public OptionChoice? SelectedOptionChoice
        {
            get => _selectedOptionChoice;
            set
            {
                if(SetProperty(ref _selectedOptionChoice, value))
                {
                    if(_selectedOptionChoice != null)
                    {
                        CurrentChoiceName = _selectedOptionChoice.Name;
                        CurrentAdditionalPrice = _selectedOptionChoice.AdditionalPrice;
                        CurrentSelectedOptionGroupId = _selectedOptionChoice.OptionGroupId;
                        CurrentIsAvailable = _selectedOptionChoice.IsAvailable;
                        CurrentDefaultQuantity = _selectedOptionChoice.DefaultQuantity;
                        CurrentMaxQuantityAllowed = _selectedOptionChoice.MaxQuantityAllowed;
                        CurrentUnitName = _selectedOptionChoice.UnitName;
                        IsEditing = true;
                    }
                    else
                    {
                        ClearForm();
                    }
                    ((RelayCommand)DeleteOptionChoiceCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)UpdateOptionChoiceCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _currentChoiceName = string.Empty;
        public string CurrentChoiceName
        {
            get => _currentChoiceName;
            set => SetProperty( ref  _currentChoiceName, value);
        }

        private decimal _currentAdditionalPrice;
        public decimal CurrentAdditionalPrice
        {
            get => _currentAdditionalPrice;
            set => SetProperty(ref _currentAdditionalPrice, value);
        }

        private Guid _currentSelectedOptionGroupId; // ComboBox'tan seçilen OptionGroup'un ID'si
        public Guid CurrentSelectedOptionGroupId
        {
            get => _currentSelectedOptionGroupId;
            set => SetProperty(ref _currentSelectedOptionGroupId, value);
        }

        private bool _currentIsAvailable = true;
        public bool CurrentIsAvailable
        {
            get => _currentIsAvailable;
            set => SetProperty(ref _currentIsAvailable, value);
        }

        private int _currentDefaultQuantity = 1;
        public int CurrentDefaultQuantity
        {
            get => _currentDefaultQuantity;
            set => SetProperty(ref _currentDefaultQuantity, value);
        }

        private int _currentMaxQuantityAllowed = 1;
        public int CurrentMaxQuantityAllowed
        {
            get => _currentMaxQuantityAllowed;
            set => SetProperty(ref _currentMaxQuantityAllowed, value);
        }

        private string? _currentUnitName;
        public string? CurrentUnitName
        {
            get => _currentUnitName;
            set => SetProperty(ref _currentUnitName, value);
        }

        private bool _isEditing = false;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        //Option grupları comboboxa yüklemek için
        private ObservableCollection<OptionGroup> _availableOptionGroups;
        public ObservableCollection<OptionGroup> AvailableOptionGroups
        {
            get => _availableOptionGroups;
            set => SetProperty(ref _availableOptionGroups, value);
        }

        //Komutlar
        public ICommand LoadDataCommand { get; }
        public ICommand AddOptionChoiceCommand { get; }
        public ICommand UpdateOptionChoiceCommand { get; }
        public ICommand DeleteOptionChoiceCommand { get; }
        public ICommand ClearFormCommand { get; }

        public OptionChoiceManagementViewModel(IOptionChoiceService optionChoiceService, IOptionGroupService optionGroupService)
        {
            _optionChoiceService = optionChoiceService ?? throw new ArgumentNullException(nameof(optionChoiceService));
            _optionGroupService = optionGroupService ?? throw new ArgumentNullException(nameof(optionGroupService));

            _optionChoices = new ObservableCollection<OptionChoice>();
            _availableOptionGroups = new ObservableCollection<OptionGroup>();

            LoadDataCommand = new RelayCommand(async (_) => await LoadAllDataAsync());
            AddOptionChoiceCommand = new RelayCommand(async (_) => await AddOptionChoiceAsync(), (_) => !IsEditing && !string.IsNullOrWhiteSpace(CurrentChoiceName) && CurrentSelectedOptionGroupId != Guid.Empty);
            UpdateOptionChoiceCommand = new RelayCommand(async (_) => await UpdateOptionChoiceAsync(), (_) => IsEditing && SelectedOptionChoice != null && !string.IsNullOrWhiteSpace(CurrentChoiceName) && CurrentSelectedOptionGroupId != Guid.Empty);
            DeleteOptionChoiceCommand = new RelayCommand(async (_) => await DeleteOptionChoiceAsync(), (_) => IsEditing && SelectedOptionChoice != null);
            ClearFormCommand = new RelayCommand((_) => ClearForm());

            _ = LoadAllDataAsync();
        }

        public async Task LoadAllDataAsync()
        {
            await LoadOptionGroupAsync();
            await LoadOptionChoiceAsync();
        }

        public async Task LoadOptionGroupAsync()
        {
            StatusMessage = "Seçenek grupları yükleniyor...";
            try
            {
                var groups = await _optionGroupService.GetAllOptionGroupAsync();
                AvailableOptionGroups.Clear();
                foreach (var group in groups)
                {
                    AvailableOptionGroups.Add(group);
                }

            }
        }
    }
}
