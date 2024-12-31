﻿using AutoMapper;
using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _database;

        public HotelRepository(AppDbContext database, IMapper mapper)
        {
            _database = database;
        }

        public async Task/*<RezervareDto>*/ AdaugaRezervare(Rezervare rezervare, int tipCameraId)
        {
            _database.Rezervari.Add(rezervare);
            //update si savechanges
            var tipCameraUpdated = (from tipCamera in _database.TipCamere
                                    where tipCamera.TipCameraId == tipCameraId
                                    select tipCamera).First();
            tipCameraUpdated.NrCamereDisponibile--;
            tipCameraUpdated.NrCamereOcupate++;
            _database.SaveChanges();

            RezervareDto rezervareDto = new RezervareDto();
            rezervareDto.PretCameraId = rezervare.PretCameraId;
            rezervareDto.UserId = rezervare.UserId;
            rezervareDto.CheckIn = rezervare.CheckIn;
            rezervareDto.CheckOut = rezervare.CheckOut;
        }

        public async Task<List<Hotel>> GetAllHotels()
        {
            var listHotels = _database.Hotels.ToList();
            return listHotels;
        }

        public async Task<List<HotelTipCameraPretReview>> GetAllHotelsByRating()
        {
            var hotelsTipCamerePretReview = (from hotels in _database.Hotels
                                       join tipCamere in _database.TipCamere
                                       on hotels.HotelId equals tipCamere.HotelId
                                       join pretCamere in _database.PretCamere
                                       on tipCamere.TipCameraId equals pretCamere.TipCameraId
                                       join reviews in _database.Reviews
                                       on hotels.HotelId equals reviews.HotelId
                                       select new HotelTipCameraPretReview
                                       {
                                           HotelName = hotels.Name,
                                           Address = hotels.Address,
                                           /*TipCameraName = tipCamere.Name,
                                           CapacitatePersoane = tipCamere.CapacitatePersoane,
                                           NrTotalCamere = tipCamere.NrTotalCamere,
                                           NrCamereDisponibile = tipCamere.NrCamereDisponibile,
                                           NrCamereOcupate = tipCamere.NrCamereOcupate,
                                           PretNoapte = pretCamere.PretNoapte,
                                           StartPretCamera = pretCamere.StartPretCamera,
                                           EndPretCamera = pretCamere.EndPretCamera,*/
                                           Rating = reviews.Rating,
                                           Description = reviews.Description
                                       }).ToList();

            return hotelsTipCamerePretReview;
        }

        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            var hotelsTipCamere =   (from hotels in _database.Hotels
                                    join tipCamere in _database.TipCamere
                                    on hotels.HotelId equals tipCamere.HotelId
                                    select new HotelTipCamera
                                    {
                                        HotelName = hotels.Name,
                                        Address = hotels.Address,
                                        TipCameraName = tipCamere.Name,
                                        CapacitatePersoane = tipCamere.CapacitatePersoane,
                                        NrTotalCamere = tipCamere.NrTotalCamere,
                                        NrCamereDisponibile = tipCamere.NrCamereDisponibile,
                                        NrCamereOcupate = tipCamere.NrCamereOcupate,
                                        TipCameraId = tipCamere.TipCameraId
                                    }).ToList();
             return hotelsTipCamere;
        }

        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret()
        {
            var hotelsTipCamerePret = (from hotels in _database.Hotels
                                   join tipCamere in _database.TipCamere
                                   on hotels.HotelId equals tipCamere.HotelId
                                   join pretCamere in _database.PretCamere
                                   on tipCamere.TipCameraId equals pretCamere.TipCameraId
                                   select new HotelTipCameraPret
                                   {
                                       HotelName = hotels.Name,
                                       Address = hotels.Address,
                                       TipCameraName = tipCamere.Name,
                                       CapacitatePersoane = tipCamere.CapacitatePersoane,
                                       NrTotalCamere = tipCamere.NrTotalCamere,
                                       NrCamereDisponibile = tipCamere.NrCamereDisponibile,
                                       NrCamereOcupate = tipCamere.NrCamereOcupate,
                                       PretNoapte = pretCamere.PretNoapte,
                                       StartPretCamera = pretCamere.StartPretCamera,
                                       EndPretCamera = pretCamere.EndPretCamera                

                                   }).ToList();

            return hotelsTipCamerePret;
        }

        public async Task<List<PretCamera>> GetAllPretCamere()
        {
            var pretCamere = _database.PretCamere.ToList(); //de ce nu ma obliga sa pun await daca scriu
            return pretCamere;                                         //_database.PretCamere.ToList()? dar ma obliga
                                                                      //sa fac metoda async ca sa nu dea eroare la return
        }

        public async Task<List<Review>> GetAllReviews()
        {
            var reviews = _database.Reviews.ToList();
            return reviews;
        }

        public async Task<List<Rezervare>> GetAllRezervariAsync()
        {
            return await _database.Rezervari
                .Include(r => r.PretCamera) // Include PretCamera data
                .ToListAsync();
        }

        public async Task<List<TipCamera>> GetAllTipCamereAsync()
        {
            return await _database.TipCamere.ToListAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = _database.Users.ToList();
            return users;
        }
    }
}
