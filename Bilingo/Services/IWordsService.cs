using Bilingo.Data;
using Bilingo.Models;
using Bilingo.Models.UserDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReversoApi;
using ReversoApi.Models;
using ReversoApi.Models.Word;

namespace Bilingo.Services
{
    public interface IWordsService
    {
        Task InitWords();
        Task SetAlreadyKnown(int userId, int wordId);
        Task<NewWordDTO> GetNewWord(int userId);
        Task SwitchToNewStage(int wordId, int userId);
    }

    public class WordsService : IWordsService
    {
        private readonly ApplicationDbContext _context;

        public WordsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task InitWords()
        {
            if (_context.Words.Count() > 2000)
            {
                throw new Exception("Table Words is already initialized");
            }
            var wordsTableFile = File.ReadAllLines("DefaultData/WordsTable.txt");
            var stringList = new List<string>(wordsTableFile);
            foreach (var curString in stringList)
            {
                var array = curString.Split('-');
                await _context.AddAsync(new Word
                {
                    Value = array[0],
                    Level = array[1]
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task SetAlreadyKnown(int userId, int wordId)
        {
            await _context.UserWords.AddAsync(new UserWord
            {
                UserId = userId,
                WordId = wordId,
                WordStatus = (int)WordStatus.AlreadyKnown
            });
            await _context.SaveChangesAsync();
        }

        public async Task<NewWordDTO> GetNewWord(int userId)
        {
            var userWords = _context.UserWords.Where(x => x.UserId == userId).Include(x => x.Word).Select(x => x.Word.Value).ToList();
            var words = _context.Words.Where(x => !userWords.Contains(x.Value)).ToList();
            var randIndex = new Random().Next(words.Count);
            var word = words[randIndex];

            var service = new ReversoService();
            TranslatedResponse result = await service.TranslateWord(new TranslateWordRequest(from: Language.En, to: Language.Ru)
            {
                Word = word.Value,
            });
            var tranlations = new List<string>();
            var examples = new List<Example>();
            
            foreach (var resultSource in result.Sources)
            {
                foreach (var translations in resultSource.Translations)
                {
                    tranlations.Add(translations.Translation);
                    foreach (var translationsContext in translations.Contexts)
                    {
                        examples.Add(new Example
                        {
                            Value = translationsContext.Source,
                            Translation = translationsContext.Target
                        });
                    }
                }
            }
            return new NewWordDTO
            {
                Id = word.Id,
                Word = word.Value,
                Level = word.Level,
                Translations = tranlations,
                Examples = examples
            };
        }

        public async Task SwitchToNewStage(int wordId, int userId)
        {
            var userWord = await _context.UserWords.FirstOrDefaultAsync(x => x.WordId == wordId && x.UserId == userId);
            if (userWord == null)
            {
                await _context.UserWords.AddAsync(new UserWord
                {
                    UserId = userId,
                    WordId = wordId,
                    WordStatus = 1,
                    NextRepetitionDateTime = DateTime.Now + RepetitionTimings.Repetitions[0],
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                if (userWord.WordStatus < 7) userWord.WordStatus += 1;
                else throw new Exception("This word has beed completely learnt");
                if (userWord.WordStatus < 7)
                    userWord.NextRepetitionDateTime = DateTime.Now + RepetitionTimings.Repetitions[userWord.WordStatus - 1];
                await _context.SaveChangesAsync();
            }
        }
    }
}
