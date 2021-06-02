using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TourPlanner.WPF.ViewModels
{
    public class ValidatedViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly ConcurrentDictionary<string, List<string>> _errors
            = new ConcurrentDictionary<string, List<string>>();
        
        private readonly object _validationLock = new object();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        
        public bool HasErrors => this._errors.Any();

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            ValidateProperty(propertyName);
        }
        
        protected virtual void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public bool HasError(string propertyName)
        {
            return this._errors.ContainsKey(propertyName);
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(propertyName))
            {
                return errors;
            }

            this._errors.TryGetValue(propertyName, out errors);
            return errors;
        }

        protected Task ValidateAsync()
        {
            lock (this._validationLock)
            {
                return Task.Run(() => Validate());
            }
        }

        protected Task ValidatePropertyAsync(string propertyName)
        {
            lock (this._validationLock) 
            {
                return Task.Run(() => ValidateProperty(propertyName));
            }
        }

        protected bool ValidateProperty(string propertyName)
        {
            var context = new ValidationContext(this);
            context.MemberName = propertyName;
            var propertyInfo = GetType().GetProperty(propertyName);
            var property = propertyInfo?.GetValue(this);

            var result = new List<ValidationResult>();
            Validator.TryValidateProperty(property, context, result);

            ClearOldErrors(result, propertyName);
            RegisterNewErrors(result);
            return !HasError(propertyName);
        }

        protected bool Validate()
        {
            var context = new ValidationContext(this);
            var result = new List<ValidationResult>();
            Validator.TryValidateObject(this, context, result, true);
            
            ClearOldErrors(result);
            RegisterNewErrors(result);
            return !this.HasErrors;
        }

        private void ClearOldErrors(ICollection<ValidationResult> result)
        {
            foreach (string property in this._errors.Keys)
            {
                ClearOldErrors(result, property);
            }
        }

        private void ClearOldErrors(ICollection<ValidationResult> result, string propertyName)
        {
            bool errorFree = result.All(validation => validation.MemberNames.All(x => x != propertyName));
            
            if (errorFree)
            {
                this._errors.TryRemove(propertyName, out _);
                OnErrorsChanged(propertyName);
            }
        }

        private void RegisterNewErrors(ICollection<ValidationResult> result)
        {
            foreach (var validation in result)
            {
                if (string.IsNullOrEmpty(validation.ErrorMessage))
                {
                    continue;
                }

                foreach (string property in validation.MemberNames)
                {
                    if (!this._errors.ContainsKey(property))
                    {
                        this._errors[property] = new List<string>();
                    }

                    this._errors[property].Add(validation.ErrorMessage);
                    OnErrorsChanged(property);
                }
            }
        }
    }
}