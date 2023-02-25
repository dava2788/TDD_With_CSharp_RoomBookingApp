using Microsoft.EntityFrameworkCore;
using RoomBookingApp.Domain;
using RoomBookingApp.Persistance.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RoomBookingApp.Persistance.Tests
{
    public class RoomBookingServiceTest
    {
        [Fact]
        public void Should_Return_Available_Rooms()
        {
            //Arrange
            var date = DateTime.Now;
            var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase("AvailableRoomTest")
                .Options;

            using var context = new RoomBookingAppDbContext(dbOptions);
            context.Rooms.Add(new Room { Id = 1, Name = "Room 1" });
            context.Rooms.Add(new Room { Id = 2, Name = "Room 2" });
            context.Rooms.Add(new Room { Id = 3, Name = "Room 3" });

            context.RoomBookings.Add(new RoomBooking { RoomId = 1, Date = date });
            context.RoomBookings.Add(new RoomBooking { RoomId = 2, Date = date.AddDays(-1) });

            context.SaveChanges();

            var roomBookingService = new RoomBookingService(context);

            //Act
            var availableRoom = roomBookingService.GetAvailableRooms(date);

            //Assert
            Assert.Equal(2, availableRoom.Count());
            Assert.Contains(availableRoom, q => q.Id == 2);
            Assert.Contains(availableRoom, q => q.Id == 3);
            Assert.DoesNotContain(availableRoom, q => q.Id == 1);

        }

        [Fact]
        public void Should_Save_Room_Booking()
        {
            var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase("ShouldSAveTest")
                .Options;

            var roomBooking = new RoomBooking { RoomId = 1, Date = DateTime.Now };

            using var context = new RoomBookingAppDbContext(dbOptions);
            var roomBookingService = new RoomBookingService(context);
            roomBookingService.Save(roomBooking);


            var bookings = context.RoomBookings.ToList();
            var booking = Assert.Single(bookings);


            //Assert
            Assert.Equal(roomBooking.Date, booking.Date);
            Assert.Equal(roomBooking.RoomId, booking.RoomId);

        }
    }
}
