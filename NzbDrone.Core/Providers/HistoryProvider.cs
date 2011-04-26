﻿using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Core.Repository;
using NzbDrone.Core.Repository.Quality;
using SubSonic.Repository;

namespace NzbDrone.Core.Providers
{
    public class HistoryProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository _repository;

        public HistoryProvider(IRepository repository)
        {
            _repository = repository;
        }

        public HistoryProvider()
        {
        }

        public virtual IQueryable<History> AllItems()
        {
            return _repository.All<History>();
        }

        public virtual void Purge()
        {
            _repository.DeleteMany(AllItems());
            Logger.Info("History has been Purged");
        }

        public virtual void Trim()
        {
            var old = AllItems().Where(h => h.Date < DateTime.Now.AddDays(-30));
            _repository.DeleteMany(old);
            Logger.Info("History has been trimmed, items older than 30 days have been removed");
        }

        public virtual void Add(History item)
        {
            _repository.Add(item);
            Logger.Debug("Item added to history: {0}", item.NzbTitle);
        }

        public virtual bool Exists(int episodeId, QualityTypes quality, bool proper)
        {
            //Looks for the existence of this episode in History
            if (_repository.Exists<History>(h => h.EpisodeId == episodeId && h.Quality == quality && h.IsProper == proper))
                return true;

            Logger.Debug("Episode not in History. ID:{0} Q:{1} Proper:{2}", episodeId, quality, proper);
            return false;
        }
    }
}