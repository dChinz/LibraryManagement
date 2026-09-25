using Microsoft.AspNetCore.Routing;

namespace Library.Models
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling(
                (double)TotalItems / PageSize
            );

        public bool HasPrevious =>
            CurrentPage > 1;

        public bool HasNext =>
            CurrentPage < TotalPages;

        // Tên parameter của trang hiện tại
        public string PageParameter { get; set; } = "page";

        // Các parameter khác cần giữ lại khi chuyển trang
        public Dictionary<string, string?> RouteValues { get; set; }
            = new Dictionary<string, string?>();
    }
}