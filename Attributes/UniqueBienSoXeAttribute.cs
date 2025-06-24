using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using bike.Models;
using bike.Repository;

namespace bike.Attributes
{
    public class UniqueBienSoXeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (BikeDbContext)validationContext.GetService(typeof(BikeDbContext));
            var bienSoXe = value as string;

            if (string.IsNullOrEmpty(bienSoXe))
                return ValidationResult.Success;

            var xe = (Xe)validationContext.ObjectInstance;
            var exists = dbContext.Xe.Any(x => x.BienSoXe == bienSoXe && x.MaXe != xe.MaXe);

            if (exists)
                return new ValidationResult("Biển số xe này đã tồn tại!");

            return ValidationResult.Success;
        }
    }
}