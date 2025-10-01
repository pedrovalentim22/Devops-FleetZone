using FluentValidation;

public class MotocicletaRequestValidator : AbstractValidator<MotocicletaRequest>
{
    public MotocicletaRequestValidator()
    {
        RuleFor(x => x.Placa).NotEmpty().Length(7, 8);
        RuleFor(x => x.Modelo).NotEmpty().MaximumLength(80);
        RuleFor(x => x.PatioId).NotEmpty(); // Guid n�o pode ser vazio
    }
}
