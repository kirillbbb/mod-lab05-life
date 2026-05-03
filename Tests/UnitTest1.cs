using Xunit;
using cli_life;

namespace Tests;

public class UnitTests
{
    [Fact]
    public void Cell_Dies_Alone()
    {
        var c = new Cell { IsAlive = true };
        c.DetermineNextLiveState();
        c.Advance();
        Assert.False(c.IsAlive);
    }

    [Fact]
    public void Board_CountAlive()
    {
        var b = new Board(3,3);
        b.Cells[0,0].IsAlive = true;
        Assert.Equal(1, b.CountAlive());
    }
}