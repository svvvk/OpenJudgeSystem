﻿namespace OJS.Web.Areas.Administration.ViewModels.Common
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;

    using OJS.Common.DataAnnotations;
    using OJS.Web.Common.Interfaces;

    public abstract class AdministrationViewModel<T> : IAdministrationViewModel<T> where T : class, new()
    {
        [ExcludeFromExcel]
        public const string EmailValidationRegularExpression = "^[A-Za-z0-9]+[\\._A-Za-z0-9-]+@([A-Za-z0-9]+[-\\.]?[A-Za-z0-9]+)+(\\.[A-Za-z0-9]+[-\\.]?[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$";

        [DatabaseProperty]
        [Display(Name = "Дата на създаване")]
        [DataType(DataType.DateTime)]
        [HiddenInput(DisplayValue = false)]
        public DateTime? CreatedOn { get; set; }

        [DatabaseProperty]
        [Display(Name = "Дата на промяна")]
        [DataType(DataType.DateTime)]
        [HiddenInput(DisplayValue = false)]
        public DateTime? ModifiedOn { get; set; }

        public virtual T GetEntityModel(T model = null)
        {
            model = model ?? new T();
            return this.ConvertToDatabaseEntity(model);
        }

        protected T ConvertToDatabaseEntity(T model)
        {
            foreach (var viewModelProperty in this.GetType().GetProperties())
            {
                var customAttributes = viewModelProperty.GetCustomAttributes(typeof(DatabasePropertyAttribute), true);

                if (customAttributes.Any())
                {
                    var name = (customAttributes.First() as DatabasePropertyAttribute).Name;
                    
                    if (string.IsNullOrEmpty(name))
                    {
                        name = viewModelProperty.Name;
                    }

                    var databaseEntityProperty = model.GetType().GetProperties().FirstOrDefault(pr => pr.Name == name);

                    if (databaseEntityProperty != null)
                    {
                        databaseEntityProperty.SetValue(model, viewModelProperty.GetValue(this));
                    }
                }
            }

            return model;
        }
    }
}