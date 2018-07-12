using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Helpers
{
    public static class EntityValidator
    {
        public static (string[] errors, bool isValid) Validate<T>(T entity) where T : class
        {
            var results = new List<ValidationResult>();
            var validator = new ValidationContext(entity, null, null);
            var isValid = Validator.TryValidateObject(entity, validator, results, true);
            var errorDictionary = new Dictionary<string,string>();
            
            var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
            return (errors: errors, isValid: isValid);
        }
    }
}
