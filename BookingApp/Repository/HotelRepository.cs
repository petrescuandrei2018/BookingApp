﻿using AutoMapper;
using BookingApp.Data;
using BookingApp.Helpers;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using BookingApp.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BookingApp.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _database;
        private readonly IServiciuCacheRedis _serviciuCacheRedis;
        private readonly IMapper _mapper;

        public HotelRepository(AppDbContext database, IMapper mapper, IServiciuCacheRedis serviciuCacheRedis)
        {
            _database = database;
            _serviciuCacheRedis = serviciuCacheRedis;
            _mapper = mapper;
        }

        public async Task<List<Hotel>> GetAllHotels()
        {
            var hotels = await _database.Hotels.ToListAsync();
            return hotels;
        }

        public async Task AdaugaRezervareAsync(Rezervare rezervare)
        {
            await _database.Rezervari.AddAsync(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task<List<Rezervare>> GetAllRezervariCompletAsync()
        {
            return await _database.Rezervari.ToListAsync();
        }

        public async Task<PretCamera> GetPretCameraByIdAsync(int pretCameraId)
        {
            return await _database.PretCamere.FirstOrDefaultAsync(pc => pc.PretCameraId == pretCameraId);
        }

        public async Task<List<Rezervare>> GetAllRezervariAsync()
        {
            return await _database.Rezervari
                .Include(r => r.PretCamera) // Include relația cu PretCamera
                .ThenInclude(pc => pc.TipCamera) // Include relația cu TipCamera
                .ThenInclude(tc => tc.Hotel) // Include relația cu Hotel
                .ToListAsync();
        }

        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            return await (from hotel in _database.Hotels
                          join tipCamera in _database.TipCamere on hotel.HotelId equals tipCamera.HotelId
                          select new HotelTipCamera
                          {
                              HotelName = hotel.Name,
                              Address = hotel.Address,
                              TipCameraName = tipCamera.Name,
                              CapacitatePersoane = tipCamera.CapacitatePersoane,
                              NrTotalCamere = tipCamera.NrTotalCamere,
                              NrCamereDisponibile = tipCamera.NrCamereDisponibile,
                              NrCamereOcupate = tipCamera.NrCamereOcupate,
                              TipCameraId = tipCamera.TipCameraId
                          }).ToListAsync();
        }


        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret()
        {
            return await (from hotel in _database.Hotels
                          join tipCamera in _database.TipCamere on hotel.HotelId equals tipCamera.HotelId
                          join pretCamera in _database.PretCamere on tipCamera.TipCameraId equals pretCamera.TipCameraId
                          select new HotelTipCameraPret
                          {
                              HotelName = hotel.Name,
                              Address = hotel.Address,
                              TipCameraName = tipCamera.Name,
                              CapacitatePersoane = tipCamera.CapacitatePersoane,
                              NrTotalCamere = tipCamera.NrTotalCamere,
                              NrCamereDisponibile = tipCamera.NrCamereDisponibile,
                              NrCamereOcupate = tipCamera.NrCamereOcupate,
                              PretNoapte = pretCamera.PretNoapte,
                              StartPretCamera = pretCamera.StartPretCamera,
                              EndPretCamera = pretCamera.EndPretCamera
                          }).ToListAsync();
        }


        public async Task<List<HotelTipCameraPretRecenzie>> GetAllHotelsByRating()
        {
            return await (from hotel in _database.Hotels
                          join tipCamera in _database.TipCamere on hotel.HotelId equals tipCamera.HotelId
                          join pretCamera in _database.PretCamere on tipCamera.TipCameraId equals pretCamera.TipCameraId
                          join recenzie in _database.Recenzii on hotel.HotelId equals recenzie.HotelId
                          select new HotelTipCameraPretRecenzie
                          {
                              NumeHotel = hotel.Name,
                              Adresa = hotel.Address,
                              NumeTipCamera = tipCamera.Name,
                              CapacitatePersoane = tipCamera.CapacitatePersoane,
                              NrTotalCamere = tipCamera.NrTotalCamere,
                              NrCamereDisponibile = tipCamera.NrCamereDisponibile,
                              NrCamereOcupate = tipCamera.NrCamereOcupate,
                              PretNoapte = pretCamera.PretNoapte,
                              StartPretCamera = pretCamera.StartPretCamera,
                              EndPretCamera = pretCamera.EndPretCamera,
                              Rating = recenzie.Rating,
                              Descriere = recenzie.Descriere, // Înlocuit Description cu Descriere
                              AspectePozitive = recenzie.AspectePozitive,
                              AspecteNegative = recenzie.AspecteNegative,
                              RecomandaHotelul = recenzie.RecomandaHotelul
                          }).ToListAsync();
        }


        public async Task<List<Recenzie>> GetAllRecenzii()
        {
            return await _database.Recenzii.ToListAsync();
        }

        public async Task<List<Rezervare>> GetNonExpiredRezervari()
        {
            return await _database.Rezervari
                .Include(r => r.PretCamera)
                .ThenInclude(pc => pc.TipCamera)
                .ThenInclude(tc => tc.Hotel)
                .Where(r => r.CheckOut > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<List<PretCamera>> GetAllPretCamere()
        {
            return await _database.PretCamere.ToListAsync();
        }

        public async Task<List<TipCamera>> GetAllTipCamereAsync()
        {
            return await _database.TipCamere.ToListAsync();
        }

        public async Task AddRezervareAsync(Rezervare rezervare)
        {
            try
            {
                _database.Rezervari.Add(rezervare);

                // Actualizează disponibilitatea camerei
                var tipCamera = await _database.TipCamere.FirstOrDefaultAsync(tc => tc.TipCameraId == rezervare.PretCamera.TipCameraId);
                if (tipCamera != null)
                {
                    tipCamera.NrCamereDisponibile--;
                    tipCamera.NrCamereOcupate++;
                }

                await _database.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la salvarea rezervării: {ex.Message}");
                throw;
            }
        }

        public async Task<Hotel?> GetHotelByNameAsync(string hotelName)
        {
            return await _database.Hotels
                .FirstOrDefaultAsync(h => h.Name == hotelName);
        }

        public async Task<TipCamera> GetTipCameraByNameAsync(string numeCamera)
        {
            return await _database.TipCamere.FirstOrDefaultAsync(tc => tc.Name == numeCamera);
        }

        public async Task<PretCamera> GetPretCameraByTipCameraIdAsync(int tipCameraId)
        {
            return await _database.PretCamere.FirstOrDefaultAsync(pc => pc.TipCameraId == tipCameraId);
        }



        public async Task<Rezervare> GetRezervareByIdAsync(int rezervareId)
        {
            return await _database.Rezervari
                .Include(r => r.User)
                .Include(r => r.PretCamera)
                .FirstOrDefaultAsync(r => r.RezervareId == rezervareId);
        }

        public async Task ActualizeazaRezervareAsync(Rezervare rezervare)
        {
            _database.Rezervari.Update(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task ActualizeazaPreturiRezervareAsync(Rezervare rezervare)
        {
            rezervare.CalculareSumaTotala();
            rezervare.SumaRamasaDePlata = rezervare.SumaTotala;

            _database.Rezervari.Update(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task ActualizeazaStarePlataAsync(int rezervareId, string starePlata)
        {
            var rezervare = await GetRezervareByIdAsync(rezervareId);
            if (rezervare == null)
            {
                throw new Exception($"Rezervarea cu ID-ul {rezervareId} nu a fost găsită.");
            }

            rezervare.StarePlata = starePlata;
            _database.Rezervari.Update(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task ActualizeazaPlataPartialaAsync(int rezervareId, decimal sumaAchitata)
        {
            var rezervare = await GetRezervareByIdAsync(rezervareId);
            if (rezervare == null)
            {
                throw new Exception($"Rezervarea cu ID-ul {rezervareId} nu a fost găsită.");
            }

            rezervare.SumaAchitata += sumaAchitata;
            rezervare.SumaRamasaDePlata = rezervare.SumaTotala - rezervare.SumaAchitata;

            if (rezervare.SumaRamasaDePlata <= 0)
            {
                rezervare.StarePlata = "Platita";
            }

            _database.Rezervari.Update(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task<Rezervare> GetRezervareByPaymentIntentAsync(string paymentIntentId)
        {
            return await _database.Rezervari.FirstOrDefaultAsync(r => r.ClientSecret == paymentIntentId);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _database.Users
                .Select(u => new User
                {
                    UserId = int.Parse(u.Id), // IdentityUser folosește `string` ca ID, trebuie convertit
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Varsta = u.Varsta,
                    Password = "****", // Nu returnăm parola reală pentru securitate
                    Rol = "user" // Dacă folosești rolurile din Identity, trebuie mapate corect
                })
                .ToListAsync();
        }

        public async Task<PretCamera?> GetPretCameraById(int pretCameraId)
        {
            return await _database.PretCamere
                .Include(pc => pc.TipCamera)
                .ThenInclude(tc => tc.Hotel)
                .FirstOrDefaultAsync(pc => pc.PretCameraId == pretCameraId);
        }

        public async Task<int> GetDisponibilitateCamera(int pretCameraId)
        {
            var tipCamera = await _database.PretCamere
                .Where(pc => pc.PretCameraId == pretCameraId)
                .Select(pc => pc.TipCamera)
                .FirstOrDefaultAsync();

            return tipCamera?.NrCamereDisponibile ?? 0;
        }

        public async Task<List<Hotel>> ObtineHoteluriPeLocatie(double latitudine, double longitudine, double razaKm)
        {
            var hoteluri = await _database.Hotels.ToListAsync();
            return hoteluri
                .Where(h => UtilitatiGeografice.CalculeazaDistanta(latitudine, longitudine, h.Latitudine, h.Longitudine) <= razaKm)
                .ToList();
        }

        public async Task<List<Hotel>> GetAllHotelsAsync()
        {
            var hoteluri = await (from hotel in _database.Hotels
                                  join tipCamera in _database.TipCamere on hotel.HotelId equals tipCamera.HotelId into camereGrup
                                  select new Hotel
                                  {
                                      HotelId = hotel.HotelId,
                                      Name = hotel.Name,
                                      Address = hotel.Address,
                                      Latitudine = hotel.Latitudine,
                                      Longitudine = hotel.Longitudine,
                                  }).ToListAsync();

            return hoteluri;
        }

    }
}
