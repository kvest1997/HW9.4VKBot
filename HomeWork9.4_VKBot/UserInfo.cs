namespace HomeWork9._4_VKBot
{
    class UserInfo
    {
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MessageId { get; set; } //Id последнего сообщения

        private string _bDay;
        public string BDay
        {
            get
            {
                if (_bDay == null)
                {
                    _bDay = "Нет информации";
                }

                return _bDay;
            }
            set
            {
                _bDay = value;
            }
        }

        public string ShortName { get; set; }

        private string _about;
        public string About
        {
            get
            {
                if (_about == null)
                {
                    _about = "Нет информации";
                }
                return _about;
            }
            set { _about = value; }
        }

        private string _sex;
        public string Sex
        {
            get
            {
                switch (_sex)
                {
                    case "1": return "Ж";
                    case "2": return "M";
                    default: return "Нет информации";
                }
            }
            set
            {
                _sex = value;
            }

        }

        private string _city;
        public string City 
        { 
            get { if (_city == null) _city = "Нет информации"; return _city; }
            set { _city = value; }
        }
        private string _country;
        public string Country 
        { 
            get { if (_country == null) _country = "Нет информации"; return _country; }
            set { _country = value; }
        }


        public UserInfo(string userId,
            string firstName,
            string lastName,
            string messageId,
            string bDay,
            string sex,
            string shortName,
            string about,
            string city,
            string country)
        {
            this.UserID = userId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MessageId = messageId;
            this._bDay = bDay;
            this._sex = sex;
            this.ShortName = shortName;
            this._about = about;
            this._city = city;
            this._country = country;
        }
    }
}
