using TaskNest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskNest.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Boards.Any())
            {
                var board = new Board
                {
                    Id = Guid.NewGuid(),
                    Name = "Project Alpha",
                    Description = "Main project board",
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem
                        {
                            Id = Guid.NewGuid(),
                            Title = "Design UI",
                            Description = "Create initial mockups",
                            CreatedAt = DateTime.UtcNow,
                            DueDate = DateTime.UtcNow.AddDays(7),
                        },
                        new TaskItem
                        {
                            Id = Guid.NewGuid(),
                            Title = "Set up CI/CD",
                            Description = "Add GitHub Actions workflow",
                            CreatedAt = DateTime.UtcNow,
                            DueDate = DateTime.UtcNow.AddDays(5),
                        }
                    }
                };

                context.Boards.Add(board);
                context.SaveChanges();

                var columns = new List<BoardColumn>
                {
                    new BoardColumn { Id = Guid.NewGuid(), Name = "To Do", Order = 1, BoardId = board.Id },
                    new BoardColumn { Id = Guid.NewGuid(), Name = "In Progress", Order = 2, BoardId = board.Id },
                    new BoardColumn { Id = Guid.NewGuid(), Name = "Done", Order = 3, BoardId = board.Id }
                };

                context.BoardColumns.AddRange(columns);
                context.SaveChanges();
            }
            else if (!context.BoardColumns.Any())
            {
                var existingBoard = context.Boards.First();

                var columns = new List<BoardColumn>
                {
                    new BoardColumn { Id = Guid.NewGuid(), Name = "To Do", Order = 1, BoardId = existingBoard.Id },
                    new BoardColumn { Id = Guid.NewGuid(), Name = "In Progress", Order = 2, BoardId = existingBoard.Id },
                    new BoardColumn { Id = Guid.NewGuid(), Name = "Done", Order = 3, BoardId = existingBoard.Id }
                };

                context.BoardColumns.AddRange(columns);
                context.SaveChanges();
            }
        }
    }
}