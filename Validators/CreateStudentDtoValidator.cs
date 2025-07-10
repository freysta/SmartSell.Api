using FluentValidation;
using SmartSell.Api.DTOs;
using System.Text.RegularExpressions;

namespace SmartSell.Api.Validators
{
    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email deve ter um formato válido")
                .MaximumLength(150).WithMessage("Email deve ter no máximo 150 caracteres");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefone é obrigatório")
                .Must(BeValidPhone).WithMessage("Telefone deve ter um formato válido (11) 99999-9999");

            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("CPF é obrigatório")
                .Must(BeValidCpf).WithMessage("CPF deve ter um formato válido");

            RuleFor(x => x.Route)
                .MaximumLength(100).WithMessage("Rota deve ter no máximo 100 caracteres");

            RuleFor(x => x.EnrollmentDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Data de matrícula não pode ser futura")
                .When(x => x.EnrollmentDate.HasValue);
        }

        private bool BeValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove formatação
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Validação do primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            int remainder = sum % 11;
            int digit1 = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cpf[9].ToString()) != digit1)
                return false;

            // Validação do segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            remainder = sum % 11;
            int digit2 = remainder < 2 ? 0 : 11 - remainder;

            return int.Parse(cpf[10].ToString()) == digit2;
        }

        private bool BeValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Remove formatação
            var cleanPhone = Regex.Replace(phone, @"[^\d]", "");

            // Verifica se tem 10 ou 11 dígitos (com ou sem 9 no celular)
            return cleanPhone.Length == 10 || cleanPhone.Length == 11;
        }
    }

    public class CreateDriverDtoValidator : AbstractValidator<CreateDriverDto>
    {
        public CreateDriverDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email deve ter um formato válido")
                .MaximumLength(150).WithMessage("Email deve ter no máximo 150 caracteres");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefone é obrigatório")
                .Must(BeValidPhone).WithMessage("Telefone deve ter um formato válido");

            RuleFor(x => x.Cnh)
                .NotEmpty().WithMessage("CNH é obrigatória")
                .Length(11).WithMessage("CNH deve ter exatamente 11 dígitos")
                .Matches(@"^\d{11}$").WithMessage("CNH deve conter apenas números");

            RuleFor(x => x.Vehicle)
                .NotEmpty().WithMessage("Veículo é obrigatório")
                .MaximumLength(100).WithMessage("Veículo deve ter no máximo 100 caracteres");

            RuleFor(x => x.LicenseExpiry)
                .GreaterThan(DateTime.Now).WithMessage("Data de vencimento da CNH deve ser futura");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Senha deve conter pelo menos uma letra minúscula, uma maiúscula e um número");
        }

        private bool BeValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var cleanPhone = Regex.Replace(phone, @"[^\d]", "");
            return cleanPhone.Length == 10 || cleanPhone.Length == 11;
        }
    }

    public class CreateRouteDtoValidator : AbstractValidator<CreateRouteDto>
    {
        public CreateRouteDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome da rota é obrigatório")
                .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres");

            RuleFor(x => x.Origin)
                .NotEmpty().WithMessage("Origem é obrigatória")
                .MaximumLength(200).WithMessage("Origem deve ter no máximo 200 caracteres");

            RuleFor(x => x.Destination)
                .NotEmpty().WithMessage("Destino é obrigatório")
                .MaximumLength(200).WithMessage("Destino deve ter no máximo 200 caracteres");

            RuleFor(x => x.DepartureTime)
                .NotEmpty().WithMessage("Horário de saída é obrigatório");

            RuleFor(x => x.ArrivalTime)
                .NotEmpty().WithMessage("Horário de chegada é obrigatório")
                .GreaterThan(x => x.DepartureTime).WithMessage("Horário de chegada deve ser posterior ao horário de saída");

            RuleFor(x => x.DriverId)
                .GreaterThan(0).WithMessage("ID do motorista deve ser válido");

            RuleFor(x => x.Capacity)
                .InclusiveBetween(1, 100).WithMessage("Capacidade deve estar entre 1 e 100 passageiros");

            RuleFor(x => x.RouteDate)
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Data da rota não pode ser no passado");
        }
    }

    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email deve ter um formato válido");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres");
        }
    }
}
