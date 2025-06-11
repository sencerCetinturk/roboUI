using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace roboUI.UI.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;//Eylemi çalıştıracak metot (nullable object parametresi)
        private readonly Predicate<object?>? _canExecute; // Eylemin çalıştırılıp çalıştırılamayacağını belirleyen metot (nullable)

        ///<summary>
        ///Her zaman çalıştırılabilen yeni bir komut oluşturur
        /// </summary>
        /// <param name="execute">Çalıştırılacak eylem.</param>
        /// 
        public RelayCommand(Action<object?> execute) : this(execute, null)
        { }

        ///<summary>
        ///Yeni bir komut oluşturur
        /// </summary>
        /// <param name="execute">Çalıştırılacak eylem</param>
        /// <param name="canExecute">Eylemin çalıştırılabilirlik durumunu belirleyen metot.</param>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        /// <summary>
        /// Komutun mevcut durumda çalıştırılıp çalıştırılamayacağını belirler.
        /// </summary>
        /// <param name="parameter">Komut için kullanılan veri. Kullanılmıyorsa null olabilir.</param>
        /// <returns>Komut çalıştırılabiliyorsa true; aksi halde false.</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Komutun hedefinde metodu çağırır.
        /// </summary>
        /// <param name="parameter">Komut için kullanılan veri. Kullanılmıyorsa null olabilir.</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// CanExecute durumunun değişebileceğini bildiren olay.
        /// WPF komut altyapısı bu olaya bağlanarak CanExecute metodunu yeniden sorgular.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// CanExecuteChanged olayını manuel olarak tetikler.
        /// Bu, CanExecute durumunun değiştiğini UI'a bildirmek için kullanılır.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
