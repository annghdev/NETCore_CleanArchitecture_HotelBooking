namespace HotelBooking.Domain.Exceptions;

public class NotFoundException(string entity, string id) : Exception($"{entity} with ID {id} not found.");
