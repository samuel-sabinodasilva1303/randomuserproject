namespace RandomUserProject.DTOs
{
    /// <summary>
    /// Autor: Samuel Sabino - 30/09/2025
    /// Descrição: Class responsavel para representar resultados paginados.
    /// </summary>
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
