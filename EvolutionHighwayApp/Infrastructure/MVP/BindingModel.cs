using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace EvolutionHighwayApp.Infrastructure.MVP
{
    public abstract class BindingModel<TBindingModel> : INotifyPropertyChanged, INotifyDataErrorInfo
            where TBindingModel : BindingModel<TBindingModel>
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

        private readonly List<PropertyValidation<TBindingModel>> validations = new List<PropertyValidation<TBindingModel>>();
        private Dictionary<string, List<string>> errorMessages = new Dictionary<string, List<string>>();

        protected BindingModel()
        {
            PropertyChanged += (s, e) => { if (e.PropertyName != "HasErrors") ValidateProperty(e.PropertyName); };
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (errorMessages.ContainsKey(propertyName)) return errorMessages[propertyName];

            return new string[0];
        }

        public bool HasErrors
        {
            get { return errorMessages.Count > 0; }
        }

        public void ValidateAll()
        {
            var propertyNamesWithValidationErrors = errorMessages.Keys;
            ClearAllErrorMessages();

            validations.ForEach(PerformValidation);
            var propertyNamesThatMightHaveChangedValidation = errorMessages.Keys.Union(propertyNamesWithValidationErrors).ToList();
            propertyNamesThatMightHaveChangedValidation.ForEach(NotifyErrorsChanged);

            NotifyPropertyChanged(m => m.HasErrors);
        }

        public void ValidateProperty(Expression<Func<TBindingModel, object>> expression)
        {
            ValidateProperty(GetPropertyName(expression));
        }

        private void ValidateProperty(string propertyName)
        {
            ClearErrorMessagesForProperty(propertyName);
            validations.Where(v => v.PropertyName == propertyName).ToList().ForEach(PerformValidation);
            NotifyErrorsChanged(propertyName);
            NotifyPropertyChanged(m => m.HasErrors);
        }

        protected PropertyValidation<TBindingModel> AddValidationFor(Expression<Func<TBindingModel, object>> expression)
        {
            var validation = new PropertyValidation<TBindingModel>(GetPropertyName(expression));
            validations.Add(validation);
            return validation;
        }

        private void NotifyErrorsChanged(string propertyName)
        {
            ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void PerformValidation(PropertyValidation<TBindingModel> validation)
        {
            if (validation.IsInvalid((TBindingModel)this))
            {
                AddErrorMessageForProperty(validation.PropertyName, validation.GetErrorMessage());
            }
        }

        private void AddErrorMessageForProperty(string propertyName, string errorMessage)
        {
            if (errorMessages.ContainsKey(propertyName))
            {
                errorMessages[propertyName].Add(errorMessage);
            }
            else
            {
                errorMessages.Add(propertyName, new List<string> { errorMessage });
            }
        }

        private void ClearAllErrorMessages()
        {
            errorMessages = new Dictionary<string, List<string>>();
        }

        private void ClearErrorMessagesForProperty(string propertyName)
        {
            errorMessages.Remove(propertyName);
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyPropertyChanged(Expression<Func<TBindingModel, object>> expression)
        {
            NotifyPropertyChanged(GetPropertyName(expression));
        }

        private static string GetPropertyName(Expression<Func<TBindingModel, object>> expression)
        {
            MemberExpression memberExpression;

            if (expression.Body is UnaryExpression)
            {
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            else
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new InvalidOperationException("You must specify a property!");
            }

            return memberExpression.Member.Name;
        }
    }
}
