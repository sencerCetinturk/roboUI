using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using roboUI.Core.Enums;
using roboUI.Core.Interfaces;
using roboUI.Core.Models;
using roboUI.UI.Commands;
using roboUI.ViewModels;

namespace roboUI.UI.ViewModels.Admin
{
    public class OptionGroupManagementViewModel : BaseViewModel
    {
        private readonly IOptionGroupService _optionGroupService;

        private ObservableCollection<OptionGroup> _optionGroups;
        public ObservableCollection<OptionGroup> OptionGroups
        {
            get => _optionGroups;
            set => SetProperty(ref _optionGroups, value);
        }

        private OptionGroup? _selectedOptionGroup;
        public OptionGroup? SelectedOptionGroup
        {
            get => _selectedOptionGroup;
            set
            {
                if (SetProperty(ref _selectedOptionGroup, value))
                {
                    // Seçili grup değiştiğinde form alanlarını doldur
                    if (_selectedOptionGroup != null)
                    {
                        CurrentOptionGroupName = _selectedOptionGroup.Name;
                        CurrentSelectionType = _selectedOptionGroup.SelectionType;
                        CurrentIsRequired = _selectedOptionGroup.IsRequired;
                        CurrentDisplayOrder = _selectedOptionGroup.DisplayOrder;
                        IsEditing = true;
                    }
                    else
                    {
                        ClearForm();
                    }
                    // Delete ve Update komutlarının çalışıp çalışamayacağını güncelle
                    ((RelayCommand)DeleteOptionGroupCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)UpdateOptionGroupCommand).RaiseCanExecuteChanged();
                }
            }
        }

        // Form alanları için property'ler
        private string _currentOptionGroupName = string.Empty;
        public string CurrentOptionGroupName
        {
            get => _currentOptionGroupName;
            set => SetProperty(ref _currentOptionGroupName, value);
        }

        private OptionSelectionType _currentSelectionType = OptionSelectionType.Single;
        public OptionSelectionType CurrentSelectionType
        {
            get => _currentSelectionType;
            set => SetProperty(ref _currentSelectionType, value);
        }

        private bool _currentIsRequired;
        public bool CurrentIsRequired
        {
            get => _currentIsRequired;
            set => SetProperty(ref _currentIsRequired, value);
        }

        private int _currentDisplayOrder;
        public int CurrentDisplayOrder
        {
            get => _currentDisplayOrder;
            set => SetProperty(ref _currentDisplayOrder, value);
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

        // Enum değerlerini ComboBox'a bağlamak için
        public Array SelectionTypes => Enum.GetValues(typeof(OptionSelectionType));


        // Komutlar
        public ICommand LoadOptionGroupsCommand { get; }
        public ICommand AddOptionGroupCommand { get; }
        public ICommand UpdateOptionGroupCommand { get; }
        public ICommand DeleteOptionGroupCommand { get; }
        public ICommand ClearFormCommand { get; }

        public OptionGroupManagementViewModel(IOptionGroupService optionGroupService)
        {
            _optionGroupService = optionGroupService ?? throw new ArgumentNullException(nameof(optionGroupService));
            _optionGroups = new ObservableCollection<OptionGroup>();

            LoadOptionGroupsCommand = new RelayCommand(async (_) => await LoadOptionGroupsAsync());
            AddOptionGroupCommand = new RelayCommand(async (_) => await AddOptionGroupAsync(), (_) => !IsEditing && !string.IsNullOrWhiteSpace(CurrentOptionGroupName));
            UpdateOptionGroupCommand = new RelayCommand(async (_) => await UpdateOptionGroupAsync(), (_) => IsEditing && SelectedOptionGroup != null && !string.IsNullOrWhiteSpace(CurrentOptionGroupName));
            DeleteOptionGroupCommand = new RelayCommand(async (_) => await DeleteOptionGroupAsync(), (_) => IsEditing && SelectedOptionGroup != null);
            ClearFormCommand = new RelayCommand((_) => ClearForm());

            //View model yüklendiğinde verileri çek
            _ = LoadOptionGroupsAsync(); //"Fire and forget" - Hata yönetimi eklenebilir
        }

        private async Task LoadOptionGroupsAsync()
        {
            StatusMessage = "Seçenek grupları yükleniyor...";

            try
            {
                var groups = await _optionGroupService.GetAllOptionGroupAsync();
                OptionGroups.Clear();
                foreach (var group in groups)
                {
                    OptionGroups.Add(group);
                }
                StatusMessage = $"{OptionGroups.Count} seçenek grubu yüklendi.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Hata: Seçenek grupları yüklenemedi. {ex.Message}";
                //Loglama
            }
        }

        async Task AddOptionGroupAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentOptionGroupName))
            {
                StatusMessage = "Grup adı boş olamaz";
                return;
            }

            var newGroup = new OptionGroup
            {
                Name = CurrentOptionGroupName,
                SelectionType = CurrentSelectionType,
                IsRequired = CurrentIsRequired,
                DisplayOrder = CurrentDisplayOrder
            };

            try
            {
                var addedGroup = await _optionGroupService.AddOptionGroupAsync(newGroup);
                OptionGroups.Add(addedGroup);
                StatusMessage = $"'{addedGroup.Name}' başarıyla eklendi.";
                ClearForm();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Hata: Seçenek grubu eklenemedi. {ex.Message}";
                //Loglama
            }
        }

        private async Task UpdateOptionGroupAsync()
        {
            if (SelectedOptionGroup == null || string.IsNullOrWhiteSpace(CurrentOptionGroupName))
            {
                StatusMessage = "Güncellenecek bir grup seçilmeli ve adı boş olamaz.";
                return;
            }

            //Güncellenecek nesneti bul ve özelliklerini güncelle
            //Veya SelectedIotşonGroup'u doğrudan güncelle (eğer referans tipiyse ve contezt izliyorsa)
            //Ama daha güvenli olanı, yeni bir nesneyle veya var olanı güncelleyerek servise göndermek.
            SelectedOptionGroup.Name = CurrentOptionGroupName;
            SelectedOptionGroup.SelectionType = CurrentSelectionType;
            SelectedOptionGroup.IsRequired = CurrentIsRequired;
            SelectedOptionGroup.DisplayOrder = CurrentDisplayOrder;
            //SelectedOptionGroup.UpdateAt = DateTime.UtcNow;

            try
            {
                await _optionGroupService.UpdateOptionGroupAsync(SelectedOptionGroup);
                StatusMessage = $"'{SelectedOptionGroup.Name}' başarıyla güncellendi.";
                // Listeyi yenilemek yerine, ObservableCollection'daki öğeyi güncellemek daha performanslı olabilir
                // Ancak basitlik için şimdilik listeyi yeniden yükleyebilir veya değişiklikleri yansıtabiliriz.
                // Veya SelectedOptionGroup zaten referansla güncellendiği için UI'da görünür.
                await LoadOptionGroupsAsync();
                ClearForm();
            }catch (Exception ex)
            {
                StatusMessage = $"Hata: Seçenek grubu güncellenemedi. {ex.Message}";    
                //Loglama
            }
        }

        private async Task DeleteOptionGroupAsync()
        {
            if(SelectedOptionGroup == null)
            {
                StatusMessage = "Silinecek bir grup seçilmedi.";
                return;
            }

            //Kullanıcıya onay sorusu(MessageBox)
            var result = MessageBox.Show($"'{SelectedOptionGroup.Name}' grubunu silmek istediğiniz emin misiniz?",
                "Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) return;

            try
            {
                await _optionGroupService.DeleteOptionGroupAsync(SelectedOptionGroup.Id);
                OptionGroups.Remove(SelectedOptionGroup);
                StatusMessage = $"'{SelectedOptionGroup.Name}' başarıyla silindi.";
                ClearForm();
            }catch(Exception ex)
            {
                StatusMessage = $"Hata: Seçenek grubu silinemedi. {ex.Message}";
            }
        }

        private void ClearForm()
        {
            CurrentOptionGroupName = string.Empty;
            CurrentSelectionType = OptionSelectionType.Single;
            CurrentIsRequired = false;
            CurrentDisplayOrder = 0;
            SelectedOptionGroup = null;
            IsEditing = false;
            //Add komutunun çalışıp çalışmayacağını güncelle
            ((RelayCommand)AddOptionGroupCommand).RaiseCanExecuteChanged();
        }
    }
}
