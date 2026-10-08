namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications
{
    public interface IDeactivatable
    {
        bool IsActive { get; }

       void Activate();

        void Deactivate();
    }
}
