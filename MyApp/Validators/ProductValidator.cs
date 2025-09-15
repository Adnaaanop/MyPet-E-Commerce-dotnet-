//using FluentValidation;
//using MyApp.DTOs.Products;

//namespace MyApp.Validators
//{
//    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
//    {
//        public CreateProductDtoValidator()
//        {
//            RuleFor(x => x.Name)
//                .NotEmpty().WithMessage("Product name is required.")
//                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

//            RuleFor(x => x.Price)
//                .GreaterThan(0).WithMessage("Price must be greater than zero.");

//            RuleFor(x => x.Stock)
//                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

//            RuleFor(x => x.ImageUrl)
//                .NotEmpty().WithMessage("Image URL is required.");
//        }
//    }

//    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
//    {
//        public UpdateProductDtoValidator()
//        {
//            RuleFor(x => x.Name)
//                .NotEmpty().WithMessage("Product name is required.")
//                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

//            RuleFor(x => x.Price)
//                .GreaterThan(0).WithMessage("Price must be greater than zero.");

//            RuleFor(x => x.Stock)
//                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
//        }
//    }
//}
