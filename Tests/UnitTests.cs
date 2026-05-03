using Xunit;
using cli_life;

namespace Tests;

public class UnitTests
{
    [Fact]
    public void Cell_Dies_Without_Neighbors()
    {
        var c = new Cell { IsAlive = true };
        c.DetermineNextLiveState();
        c.Advance();
        Assert.False(c.IsAlive);
    }

    [Fact]
    public void Cell_Survives_With_2_Neighbors()
    {
        var c = new Cell { IsAlive = true };
        c.neighbors.Add(new Cell { IsAlive = true });
        c.neighbors.Add(new Cell { IsAlive = true });

        c.DetermineNextLiveState();
        c.Advance();

        Assert.True(c.IsAlive);
    }

    [Fact]
    public void Cell_Survives_With_3_Neighbors()
    {
        var c = new Cell { IsAlive = true };
        for (int i = 0; i < 3; i++)
            c.neighbors.Add(new Cell { IsAlive = true });

        c.DetermineNextLiveState();
        c.Advance();

        Assert.True(c.IsAlive);
    }

    [Fact]
    public void Cell_Dies_From_Overpopulation()
    {
        var c = new Cell { IsAlive = true };
        for (int i = 0; i < 4; i++)
            c.neighbors.Add(new Cell { IsAlive = true });

        c.DetermineNextLiveState();
        c.Advance();

        Assert.False(c.IsAlive);
    }

    [Fact]
    public void Dead_Cell_Becomes_Alive_With_3_Neighbors()
    {
        var c = new Cell();
        for (int i = 0; i < 3; i++)
            c.neighbors.Add(new Cell { IsAlive = true });

        c.DetermineNextLiveState();
        c.Advance();

        Assert.True(c.IsAlive);
    }

    [Fact]
    public void Board_CountAlive_Works()
    {
        var b = new Board(3, 3);
        b.Cells[0, 0].IsAlive = true;
        b.Cells[1, 1].IsAlive = true;

        Assert.Equal(2, b.CountAlive());
    }

    [Fact]
    public void Board_Empty_Has_Zero_Alive()
    {
        var b = new Board(3, 3);
        Assert.Equal(0, b.CountAlive());
    }

    [Fact]
    public void Board_Randomize_All_Alive()
    {
        var b = new Board(3, 3);
        b.Randomize(1.0);

        Assert.Equal(9, b.CountAlive());
    }

    [Fact]
    public void Board_Randomize_All_Dead()
    {
        var b = new Board(3, 3);
        b.Randomize(0.0);

        Assert.Equal(0, b.CountAlive());
    }

    [Fact]
    public void Advance_Changes_State()
    {
        var b = new Board(3, 3);

        b.Cells[1, 0].IsAlive = true;
        b.Cells[1, 1].IsAlive = true;
        b.Cells[1, 2].IsAlive = true;

        b.Advance();

        Assert.True(b.Cells[0, 1].IsAlive);
    }

    [Fact]
    public void Single_Cell_Dies_After_Step()
    {
        var b = new Board(3, 3);
        b.Cells[1, 1].IsAlive = true;

        b.Advance();

        Assert.False(b.Cells[1, 1].IsAlive);
    }

    [Fact]
    public void Stable_Block_Remains_Stable()
    {
        var b = new Board(4, 4);

        b.Cells[1, 1].IsAlive = true;
        b.Cells[1, 2].IsAlive = true;
        b.Cells[2, 1].IsAlive = true;
        b.Cells[2, 2].IsAlive = true;

        b.Advance();

        Assert.True(b.Cells[1, 1].IsAlive);
        Assert.True(b.Cells[1, 2].IsAlive);
        Assert.True(b.Cells[2, 1].IsAlive);
        Assert.True(b.Cells[2, 2].IsAlive);
    }

    [Fact]
    public void Board_Size_Correct()
    {
        var b = new Board(5, 7);
        Assert.Equal(5, b.Columns);
        Assert.Equal(7, b.Rows);
    }

    [Fact]
    public void Neighbors_Count_Is_8()
    {
        var b = new Board(3, 3);
        Assert.Equal(8, b.Cells[1, 1].neighbors.Count);
    }

    [Fact]
    public void Advance_Does_Not_Throw()
    {
        var b = new Board(10, 10);
        b.Advance();

        Assert.True(true);
    }
}