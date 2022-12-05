using System.Collections.Generic;

namespace CustomIdentityApp.ViewModels
{
    public class MarkViewModel
    {
        public List<RatingViewModel> marks { get; set; }

        public MarkViewModel()
        {
            marks = new List<RatingViewModel>();
        }
    }
}
