namespace BlastSharp.Either;

public class Either<TLeft, TRight>
    where TLeft : class
    where TRight : class
{
    private TLeft? LeftValue { get; set; }
    private bool IsLeftValue { get; set; }
    private TRight? RightValue { get; set; }
    private bool IsRightValue { get; set; }

    public static Either<TLeft, TRight> Create(TLeft left)
    {
        return new Either<TLeft, TRight>
        {
            LeftValue = left,
            IsLeftValue = true,
            RightValue = default!,
            IsRightValue = false,
        };
    }

    public static Either<TLeft, TRight> Create(TRight right)
    {
        return new Either<TLeft, TRight>
        {
            LeftValue = default!,
            IsLeftValue = false,
            RightValue = right,
            IsRightValue = true
        };
    }

    public bool IsLeft()
    {
        return IsLeftValue;
    }

    public bool IsRight()
    {
        return IsRightValue;
    }

    public TLeft? Left
    {
        get => IsLeftValue ? LeftValue : null;
        set
        {
            IsLeftValue = true;
            LeftValue = value;
            IsRightValue = false;
            RightValue = default!;
        }
    }

    public TRight? Right
    {
        get => IsRightValue ? RightValue : null;
        set
        {
            IsLeftValue = false;
            LeftValue = default!;
            IsRightValue = true;
            RightValue = value;
        }
    }
}