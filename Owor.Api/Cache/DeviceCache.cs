using System;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owor.Api.Configuration;
using Owor.Core;
using Owor.Core.OwBase;
using Owor.Shared;

namespace Owor.Api.Cache
{

    public class DeviceCache : IDeviceCache
    {

        private readonly ILogger<DeviceCache> _logger;
        private readonly IMemoryCache _deviceCache;
        private readonly IOptions<CacheOptions> _cacheOptions;
        private readonly IOwAccessor _owAccessor;

        private const string _CACHE_KEY = "DEVICES";

        public DeviceCache(ILogger<DeviceCache> logger, IMemoryCache memoryCache, IOptions<CacheOptions> cacheOptions, IOwAccessor owAccessor)
        {
            _logger = logger;
            _deviceCache = memoryCache;
            _cacheOptions = cacheOptions;
            _owAccessor = owAccessor;

            InitializeCache();
        }

        public OwDeviceDto GetDevice(string deviceId)
        {
            _logger.LogDebug("Trying to retrieve device {0}", deviceId);
            
            if (_deviceCache.TryGetValue(_CACHE_KEY, out OwDeviceDto[] cachedDevices))
            {
                return cachedDevices.Single(d => d.Id == deviceId);
            }

            InitializeCache();
            return GetDevice(deviceId);
        }

        public OwDeviceDto GetDeviceUncached(string deviceId)
        {
            _logger.LogDebug("Trying to retrieve device {0} (uncached)", deviceId);
            
            var device = _owAccessor.GetDevice(deviceId);

            _deviceCache.TryGetValue(_CACHE_KEY, out OwDeviceDto[] cachedDevices);
            var i = Array.FindIndex(cachedDevices, d => d.Id == device.Id);
            cachedDevices[i] = device;

            return device;
        }

        public OwDeviceDto[] GetDevices()
        {
            _logger.LogDebug("Trying to retrieve devices");
            
            if (_deviceCache.TryGetValue(_CACHE_KEY, out OwDeviceDto[] cachedDevices))
            {
                return cachedDevices;
            }

            InitializeCache();
            return GetDevices();
        }

        public OwDeviceDto[] GetDevicesUncached()
        {
            _logger.LogDebug("Trying to retrieve devices (uncached)");
            
            InitializeCache();
            return GetDevices();
        }

        private void InitializeCache()
        {
            _logger.LogInformation("Initializing device cache with cache options sli exp '{0}' and abs exp '{1}'", _cacheOptions.Value.SlidingExpiration, _cacheOptions.Value.AbsoluteExpiration);

            var devices = _owAccessor.GetDevices();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(_cacheOptions.Value.SlidingExpiration))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(_cacheOptions.Value.AbsoluteExpiration))
                .RegisterPostEvictionCallback(callback: CacheEvictionCallback, state: this);

            _deviceCache.Set(_CACHE_KEY, devices, cacheEntryOptions);
        }

        private void CacheEvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            _logger.LogInformation("Cleaning cache {0}: {1}", key, reason);
        }

    }

}