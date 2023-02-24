using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Models;

namespace RoomBookingApp.Core.Processors
{
    public class RoomBookingRequestProcessor
    {
        private readonly IRoomBookingService _roomBooklingService;

        public RoomBookingRequestProcessor(IRoomBookingService roomBooklingService)
        {
            _roomBooklingService = roomBooklingService;
        }

        public RoomBookingResult BookRoom(RoomBookingRequest bookingRequest)
        {
            if (bookingRequest is null)
            {
                throw new ArgumentNullException(nameof(bookingRequest));
            }

            var availableRooms = _roomBooklingService.GetAvailableRooms(bookingRequest.Date);
            var result = CreateRoomBookingbject<RoomBookingResult>(bookingRequest);

            if (availableRooms.Any())
            {
                var room = availableRooms.First();
                var roomBooking = CreateRoomBookingbject<RoomBooking>(bookingRequest);
                roomBooking.RoomId = room.Id;
                //Save the Method
                _roomBooklingService.Save(roomBooking);

                result.RoomBookingId = roomBooking.Id;
                result.Flag = BookingResultFlag.Success;
            }
            else
            {
                result.Flag = BookingResultFlag.Failure;
            }
                
            return result;
        }

        private static TRoomBooking CreateRoomBookingbject<TRoomBooking>(RoomBookingRequest bookingRequest) 
            where TRoomBooking :RoomBookingBase, new()
        {
            return new TRoomBooking
            {
                FullName = bookingRequest.FullName,
                Date = bookingRequest.Date,
                Email = bookingRequest.Email
            };
        }
    }
}