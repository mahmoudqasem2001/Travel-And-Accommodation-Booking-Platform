namespace Domain.Exceptions;

public class OverlappingDiscountIntervalsException(string message) : ConflictException(message)
{
    public override string Title => "Conflicting discount activation date intervals";
}
