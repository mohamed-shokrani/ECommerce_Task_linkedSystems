using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Attributes;
public class MaxFileSizeAttribute : ValidationAttribute
{

    private readonly int _maxFilesize;

    public MaxFileSizeAttribute(int maxFilesize)
    {
        _maxFilesize = maxFilesize;
    }
    protected override ValidationResult? IsValid
        (object? value, ValidationContext validationContext)
    {
        var file = value as FormFile;

        if (file is not null)
        {

            if (file.Length > _maxFilesize)
            {
                double filesize = (double)(file.Length) / (1024 * 1024);

                return new ValidationResult($"Maximum allowed size is  {_maxFilesize/1024/1024} MB bytes your uploaded file is " +
                    $" {Math.Round(filesize, 2)} MB .");
            }
        }
        return ValidationResult.Success;
    }
}