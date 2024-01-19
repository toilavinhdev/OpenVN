namespace SharedKernel.Application
{
    public class BaseResponse
    {
        protected string _status = "success";

        public string Status
        {
            get
            {
                if (_status == "success" && Error != null)
                {
                    _status = "error";
                }
                return _status;
            }
            set { _status = value; }
        }

        public Error Error { get; set; }

        public BaseResponse()
        {
        }

        public BaseResponse(Error error)
        {
            Error = error;
        }

        public BaseResponse(string status, Error error)
        {
            Status = status;
            Error = error;
        }
    }
}
