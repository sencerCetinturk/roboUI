using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace roboUI.ViewModels
{
    public abstract class BaseViewModel :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;//Nullable referans tipi için ? eklendi

        ///<summary>
        ///Bir özelliğin değeri değiştiğinde PropertyChanged olayını tetikler.
        ///</summary>
        ///<param name="propertyName">Değeri değişen özelliğin adı.
        ///[CallerMemberName] attribute'u sayesinde bu parametre genellikle otomatik olarak doldurulur.</param>
        ///
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ///<summary>
        ///Bir özelliğin değerini ayarlar ve ğer değer değiştirse PropertyChanged olayı tetikler
        ///Bu, bilerplate kodu azaltmak için kullanışlı bir yardımcı metottur.
        /// </summary>
        /// <typeparam name="T">Özelliğin tipi.</typeparam>
        /// <param name="storage">Özelliğin değerini tutan backing field (referansla geçirilir).</param>
        /// <param name="value">Özelliğe atanacak yeni değer.</param>
        /// <param name="propertyName">Değeri değişen özelliğin adı. otomatik olarak doldurulur.</param>
        /// <returns>Değer değiştirse true, aksi halde false.</returns>
        /// 
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string? propertyName = null)
        {
            if(Equals(storage, value)) return false; //Değer aynı, değişiklik yok, olay tetiklenmeyecek.

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName]string? propertyName = null)
        {
            if(Equals(storage, value)) return false;
            storage = value;
            onChanged?.Invoke(); // Ek eylemi çağırır.
            OnPropertyChanged(propertyName);
             return true;
        }
    }
}
