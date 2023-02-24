using Moq;
using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Shouldly;

namespace RoomBookingApp.Core
{
    public class RoomBookingRequestProcessorTest
    {
        private RoomBookingRequestProcessor _processor;
        private RoomBookingRequest _request;
        private Mock<IRoomBookingService> _roomBookingServiceMock;
        private List<Room> _availableRooms;
        public RoomBookingRequestProcessorTest()
        {

            //Global Arrrange
            //text request
            _request = new RoomBookingRequest
            {
                FullName = "Test Name",
                Email = "test@request.com",
                Date = new DateTime(2023, 02, 24)
            };
            //
            _availableRooms = new List<Room>() { new Room() { Id=1} };

            //Mock Service
            _roomBookingServiceMock = new Mock<IRoomBookingService>();
            _roomBookingServiceMock.Setup(x => x.GetAvailableRooms(_request.Date))
                .Returns(_availableRooms);
            //set up Processor object to take the mock object
            _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);
        }

        [Fact]
        public void Should_Return_Room_Booking_Response_With_Request_Values()
        {
            //Arrrange
            //Those are define in the constructor

            //Act
            RoomBookingResult result = _processor.BookRoom(_request);

            //Assert
            //Assert.NotNull(result);
            //Assert.Equal(request.FullName, result.FullName);
            //Assert.Equal(request.Email, result.FullName);
            //Assert.Equal(request.Date, result.Date);

            //Shouldly
            result.ShouldNotBeNull();
            result.FullName.ShouldBe(_request.FullName);
            result.Email.ShouldBe(_request.Email);
            result.Date.ShouldBe(_request.Date);

        }

        [Fact]
        public void Should_Return_Throw_Exception_For_Null_Request()
        {
            //Assert
            //Assert.Throws<ArgumentNullException>(() => _processor.BookRoom(null));

            //Shouldly
            var execption= Should.Throw<ArgumentNullException>(() => _processor.BookRoom(null));
            execption.ParamName.ShouldBe("bookingRequest");
   
        }

        [Fact]
        public void Should_Save_Room_Booking_Request()
        {
            RoomBooking savedBooking = null;

            _roomBookingServiceMock.Setup(q => q.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(booking =>
                {
                    savedBooking = booking;
                });

            _processor.BookRoom(_request);

            _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Once);

            savedBooking.ShouldNotBeNull();
            savedBooking.FullName.ShouldBe(_request.FullName);
            savedBooking.Email.ShouldBe(_request.Email);
            savedBooking.Date.ShouldBe(_request.Date);
            savedBooking.RoomId.ShouldBe(_availableRooms.First().Id);
        }

        [Fact]
        public void Should_Not_Save_Room_Booking_Request_If_None_Available()
        {
            _availableRooms.Clear();
            _processor.BookRoom(_request);
            _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Never);
        }

        //Data Driven Test
        [Theory]
        [InlineData(BookingResultFlag.Failure, false)]
        [InlineData(BookingResultFlag.Success,true)]
        public void Should_Return_SuccessFailure_Flag_In_Result(BookingResultFlag bookingSuccessFlag, bool isAvailable)
        {
            if(!isAvailable)
            {
                _availableRooms.Clear();
            }

            var result = _processor.BookRoom(_request);
            bookingSuccessFlag.ShouldBe(result.Flag);


        }

        [Theory]
        [InlineData(1,true)]
        [InlineData(null,false)]
        public void Should_Return_RoomBooking_Id_Result(int? roomBookingId, bool isAvailable)
        {
            if (!isAvailable)
            {
                _availableRooms.Clear();
            }
            else
            {
                _roomBookingServiceMock.Setup(q => q.Save(It.IsAny<RoomBooking>()))
                    .Callback<RoomBooking>(booking =>
                {
                    booking.Id = roomBookingId.Value;
                });
            }

            var result = _processor.BookRoom(_request);
            result.RoomBookingId.ShouldBe(roomBookingId);

        }
    }
    
    
}
