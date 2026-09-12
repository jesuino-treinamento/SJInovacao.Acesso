using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Representa um arquivo anexado a um produto
    /// </summary>
    public class Attachment : BaseEntity
    {
        public byte[] Content { get; private set; } = Array.Empty<byte>();
        public string Extension { get; private set; } = string.Empty;
        public Guid ProductId { get; private set; }
        public required Product Product { get; set; }

        protected Attachment()
        {
            // Construtor protegido para ORM
        }

        /// <summary>
        /// Cria um novo anexo com conteúdo e extensão
        /// </summary>
        public Attachment(byte[] content, string extension)
        {
            if (content == null || content.Length == 0)
                throw new ArgumentException("Conteúdo do arquivo não pode ser vazio", nameof(content));

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Extensão do arquivo não pode ser vazia", nameof(extension));

            Content = content;
            Extension = extension;
        }

        /// <summary>
        /// Atualiza o conteúdo do arquivo
        /// </summary>
        public void UpdateContent(byte[] newContent)
        {
            if (newContent == null || newContent.Length == 0)
                throw new ArgumentException("Conteúdo do arquivo não pode ser vazio", nameof(newContent));

            Content = newContent;
        }

        /// <summary>
        /// Atualiza a extensão do arquivo
        /// </summary>
        public void UpdateExtension(string newExtension)
        {
            if (string.IsNullOrWhiteSpace(newExtension))
                throw new ArgumentException("Extensão do arquivo não pode ser vazia", nameof(newExtension));

            Extension = newExtension;
        }
    }
}