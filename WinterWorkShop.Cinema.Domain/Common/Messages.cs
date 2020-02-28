namespace WinterWorkShop.Cinema.Domain.Common
{
    public static class Messages
    {
        #region Users

        #endregion

        #region Payments
        public const string PAYMENT_CREATION_ERROR = "Connection error, occured while creating new payment, please try again";
        #endregion

        #region Auditoriums
        public const string AUDITORIUM_GET_ALL_AUDITORIUMS_ERROR = "Error occured while getting all auditoriums, please try again.";
        public const string AUDITORIUM_PROPERTIE_NAME_NOT_VALID = "The auditorium Name cannot be longer than 50 characters.";
        public const string AUDITORIUM_PROPERTIE_SEATROWSNUMBER_NOT_VALID = "The auditorium number of seats rows must be between 1-20.";
        public const string AUDITORIUM_PROPERTIE_SEATNUMBER_NOT_VALID = "The auditorium number of seats number must be between 1-20.";
        public const string AUDITORIUM_CREATION_ERROR = "Error occured while creating new auditorium, please try again.";
        public const string AUDITORIUM_SEATS_CREATION_ERROR = "Error occured while creating seats for auditorium, please try again.";
        public const string AUDITORIUM_SAME_NAME = "Cannot create new auditorium, auditorium with same name alredy exist.";
        public const string AUDITORIUM_UNVALID_CINEMAID = "Cannot create new auditorium, auditorium with given cinemaId does not exist.";
        public const string AUDITORIUM_DOES_NOT_EXIST = "Auditorium does not exist.";
        public const string AUDITORIUM_DELETION_ERROR = "Unable to delete auditorium, please make sure there are no upcoming projections and then try again. ";
        public const string AUDITORIUM_NOT_FOUND = "Unable to find auditorium. ";
        public const string AUDITORIUM_UPDATE_ERROR = "Unable to update auditorium, please make sure there no upcoming projections and then try again. ";

        #endregion

        #region Cinemas
        public const string CINEMA_GET_ALL_CINEMAS_ERROR = "Error occured while getting all cinemas, please try again";
        public const string CINEMA_DOES_NOT_EXIST = "Cinema does not exist.";
        public const string CINEMA_PROPERTIE_NAME_NOT_VALID = "The cinema name cannot be longer than 255 characters.";
        public const string CINEMA_CREATION_ERROR = "Error occured while creating new cinema, please try again.";
        public const string CINEMA_GET_BY_ID = "Error occured while getting cinema by Id, please try again.";
        public const string CINEMA_SAME_NAME = "Cannot create new cinema, cinema with same name alredy exist.";
        public const string CINEMA_DELETION_ERROR = "Cannot delete cinema as one or more auditoriums has at least one projection scheduled in the future. ";
        public const string CINEMA_NOT_FOUND = "Unable to find cinema, please try again. ";



        #endregion

        #region Movies        
        public const string MOVIE_DOES_NOT_EXIST = "Movie does not exist.";
        public const string MOVIE_PROPERTIE_TITLE_NOT_VALID = "The movie title cannot be longer than 50 characters.";
        public const string MOVIE_PROPERTIE_YEAR_NOT_VALID = "The movie year must be between 1895-2100.";
        public const string MOVIE_PROPERTIE_RATING_NOT_VALID = "The movie rating must be between 1-10.";
        public const string MOVIE_CREATION_ERROR = "Error occured while creating new movie, please try again.";
        public const string MOVIE_GET_ALL_CURRENT_MOVIES_ERROR = "Error occured while getting current movies, please try again.";
        public const string MOVIE_GET_BY_ID = "Error occured while getting movie by Id, please try again.";
        public const string MOVIE_GET_ALL_MOVIES_ERROR = "Error occured while getting all movies, please try again.";
        public const string MOVIE_CURRENT_UPDATE_ERROR = "Error occured while updating current movie status, please try again.";
        public const string MOVIE_CURRENT_TO_NOT_CURRENT_UPDATE_ERROR = "Error occured while updating current movie status. This movie has projection in future, so it can not be not current.";
        public const string MOVIE_WITH_THIS_DESCRIPTION_DOES_NOT_EXIST = "There is not movie that match this description, try something new.";

        public const string BREAKPOINT = "It breaks here";
        #endregion

        #region Projections
        public const string PROJECTION_GET_ALL_PROJECTIONS_ERROR = "Error occured while getting all projections, please try again.";
        public const string PROJECTION_CREATION_ERROR = "Error occured while creating new projection, please try again.";
        public const string PROJECTIONS_AT_SAME_TIME = "Cannot create new projection, there are projections at same time alredy.";
        public const string PROJECTION_IN_PAST = "Projection time cannot be in past.";
        public const string PROJECTION_SEARCH_ERROR = "Please enter search parameter";
        public const string PROJECTION_SEARCH_NORESULT = "Search returned with no results. Please try with different search parameter. ";
        public const string PROJECTION_SEARCH_SUCCESSFUL = "Search successful. ";
        public const string PROJECTION_IN_FUTURE = "Cannot delete projection as it is scheduled in the future. ";

        #endregion

        #region Seats
        public const string SEAT_GET_ALL_SEATS_ERROR = "Error occured while getting all seats, please try again.";
        #endregion

        #region User
        public const string USER_NOT_FOUND = "User does not exist.";
        #endregion

        #region Ticket
        public const string TICKET_CREATION_ERROR = "Error occured while creating new ticket, please try again.";
        public const string TICKET_NOT_FOUND = "Error occured while finding ticket, please try again.";
        public const string TICKET_UPDATE_ERROR = "Error occured while updating ticket pay property, please try again.";
        public const string MOVIE_SEARCH_SUCCESSFUL = "Search successful. ";

        #endregion
    }
}