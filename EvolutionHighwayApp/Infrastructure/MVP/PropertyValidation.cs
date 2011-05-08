using System;

namespace EvolutionHighwayApp.Infrastructure.MVP
{
    public class PropertyValidation<TBindingModel> 
        where TBindingModel : BindingModel<TBindingModel>
    {
        private Func<TBindingModel, bool> validationCriteria;
        private string errorMessage;
        private readonly string propertyName;

        public PropertyValidation(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public PropertyValidation<TBindingModel> WithMessage(string errorMessage)
        {
            if (this.errorMessage != null)
            {
                throw new InvalidOperationException("You can only set the message once.");
            }

            this.errorMessage = errorMessage;
            return this;
        }

        public PropertyValidation<TBindingModel> When(Func<TBindingModel, bool> validationCriteria)
        {
            if (this.validationCriteria != null)
            {
                throw new InvalidOperationException("You can only set the validation criteria once.");
            }

            this.validationCriteria = validationCriteria;
            return this;
        }

        public bool IsInvalid(TBindingModel presentationModel)
        {
            if (validationCriteria == null)
            {
                throw new InvalidOperationException("No criteria have been provided for this validation. (Use the 'When(..)' method.)");
            }

            return validationCriteria(presentationModel);
        }

        public string GetErrorMessage()
        {
            if (errorMessage == null)
            {
                throw new InvalidOperationException("No error message has been set for this validation. (Use the 'WithMessage(..)' method.)");
            }

            return errorMessage;
        }

        public string PropertyName
        {
            get { return propertyName; }
        }
    }
}
