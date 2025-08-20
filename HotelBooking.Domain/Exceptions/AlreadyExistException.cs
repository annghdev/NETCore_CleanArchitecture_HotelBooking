namespace HotelBooking.Domain.Exceptions;

public class AlreadyExistException(string entity, string name) : Exception($"{entity} with Name {name} already exists.");