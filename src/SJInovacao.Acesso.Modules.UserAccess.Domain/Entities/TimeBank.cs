using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

/// <summary>
/// Classe que representa um banco de horas para colaboradores
/// </summary>
public class TimeBank : BaseEntity, IDeactivatable
{
    public Guid EmployeeId { get; private set; }             // ID do colaborador
    public Employee Employee { get; private set; } = null!; // Navegação para colaborador
    public decimal HoursBalance { get; private set; }       // Saldo de horas acumuladas
    public bool IsActive { get; private set; }                // Status ativo/inativo

    /// <summary>
    /// Construtor protegido para EF Core
    /// </summary>
    protected TimeBank() { }

    /// <summary>
    /// Cria um novo banco de horas para um colaborador
    /// </summary>
    /// <param name="employee">Colaborador (não pode ser nulo)</param>
    /// <exception cref="ArgumentNullException">Se o colaborador for nulo</exception>
    public TimeBank(Employee employee)
    {
        Employee = employee ?? throw new ArgumentNullException(nameof(employee));
        IsActive = true;
        HoursBalance = 0;
    }

    /// <summary>
    /// Adiciona horas extras ao saldo
    /// </summary>
    /// <param name="hours">Quantidade de horas a adicionar (deve ser positiva)</param>
    /// <exception cref="ArgumentException">Se horas for menor ou igual a zero</exception>
    public void AddOvertimeHours(decimal hours)
    {
        if (hours <= 0) throw new ArgumentException("Horas extras devem ser positivas.");
        HoursBalance += hours;
    }

    /// <summary>
    /// Desconta horas do saldo
    /// </summary>
    /// <param name="hours">Quantidade de horas a descontar (deve ser positiva)</param>
    /// <exception cref="ArgumentException">Se horas for menor ou igual a zero</exception>
    /// <exception cref="InvalidOperationException">Se saldo for insuficiente</exception>
    public void SubtractHours(decimal hours)
    {
        if (hours <= 0) throw new ArgumentException("Horas a serem descontadas devem ser positivas.");
        if (hours > HoursBalance) throw new InvalidOperationException("Saldo de horas insuficiente.");
        HoursBalance -= hours;
    }

    /// <summary>
    /// Desativa o banco de horas
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Ativa o banco de horas
    /// </summary>
    public void Activate() => IsActive = true;
}