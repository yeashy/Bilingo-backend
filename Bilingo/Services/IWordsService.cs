using Bilingo.Data;
using Bilingo.Models;
using Bilingo.Models.UserDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ReversoApi;
using ReversoApi.Models;
using ReversoApi.Models.Word;
using System.Text.Json;

namespace Bilingo.Services
{
    public interface IWordsService
    {
        Task InitWords();

        Task SetAlreadyKnown(int userId, int wordId);
        Task SwitchToNewStage(int wordId, int userId);

        Task<WordDTO> GetNewWord(int userId);
        Task<WordDTO> GetWordToRepeat(int userId);

        Task<ExerciseDTO> GetRandomExercise(int wordId);
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

        public async Task<WordDTO> GetNewWord(int userId)
        {
            var userWords = _context.UserWords.Where(x => x.UserId == userId).Include(x => x.Word).Select(x => x.Word.Value).ToList();
            var words = _context.Words.Where(x => !userWords.Contains(x.Value)).ToList();
            if (words.Count == 0) throw new Exception("No more new words left");

            var randIndex = new Random().Next(words.Count);
            var word = words[randIndex];

            return await GetWordDTO(word);
        }

        public async Task<WordDTO> GetWordToRepeat(int userId)
        {
            var userWords = _context.UserWords
                .Where(x => x.UserId == userId && x.WordStatus != (int)WordStatus.CompletelyLearnt && x.WordStatus != (int)WordStatus.AlreadyKnown)
                .Include(x => x.Word)
                .ToList();
            
            
            if (userWords.Count == 0) throw new Exception("No words to repeat. Suggest user to learn new words");

            var randIndex = new Random().Next(userWords.Count);
            var userWord = userWords[randIndex];
            var wordDTO = await GetWordDTO(userWord.Word);
            var wordRepetitionDTO = new WordRepetitionDTO
            {
                Id = wordDTO.Id,
                Word = wordDTO.Word,
                Phonetic = wordDTO.Phonetic,
                Level = wordDTO.Level,
                Translations = wordDTO.Translations,
                Examples = wordDTO.Examples,
                Stage = userWord.WordStatus,
                PartsOfSpeech = wordDTO.PartsOfSpeech
            };
            return wordRepetitionDTO;
        }

        private async Task<WordDTO> GetWordDTO(Word word)
        {
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

            var info = await GetInfoFromDictionaryAPI(word);
            var partsOfSpeech = info?.Meanings.Select(x => x.PartOfSpeech).ToList();

            return new WordDTO
            {
                Id = word.Id,
                Word = word.Value,
                Phonetic = info?.Phonetic,
                Level = word.Level,
                Translations = tranlations,
                Examples = examples,
                PartsOfSpeech = partsOfSpeech ?? new List<string>()
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
                    WordStatus = (int)WordStatus.Stage1,
                    NextRepetitionDateTime = DateTime.Now + RepetitionTimings.Repetitions[0],
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                if (userWord.WordStatus < (int)WordStatus.CompletelyLearnt) userWord.WordStatus += 1;
                else throw new Exception("This word has beed completely learnt");

                if (userWord.WordStatus < (int)WordStatus.CompletelyLearnt)
                {
                    userWord.NextRepetitionDateTime = DateTime.Now + RepetitionTimings.Repetitions[userWord.WordStatus - 1];
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ExerciseDTO> GetRandomExercise(int wordId)
        {
            var rand = new Random().Next(3);
            return rand switch
            {
                0 => await GenerateTask1(wordId),
                1 => await GenerateTask2(wordId),
                2 => await GenerateTask3(wordId),
                _ => await GenerateTask1(wordId),
            };
        }

        private async Task<ExerciseType1DTO> GenerateTask1(int wordId)
        {
            var word = await _context.Words.FirstOrDefaultAsync(x => x.Id == wordId);
            if (word == null) throw new Exception("Word does not exist");

            var sentence = await GetRandomSentence(word);
            var sentenceOnlyWords = sentence.Trim().Replace(".", "").Replace(",", "").Replace("-", "").Replace("?", "").Replace("!", "")
                .Replace("   ", " ").Replace("  ", " ");

            var sentenceArray = sentenceOnlyWords.Split(" ").ToList();
            var sentenceArrayRandomOrder = new List<string>();
            while (sentenceArray.Count > 0)
            {
                var randElem = sentenceArray[new Random().Next(sentenceArray.Count)];
                sentenceArrayRandomOrder.Add(randElem);
                sentenceArray.Remove(randElem);
            }
            return new ExerciseType1DTO
            {
                Type = "Type1",
                Sentence = sentence,
                SentenceRandomOrder = sentenceArrayRandomOrder
            };
        }

        private async Task<ExerciseType2DTO> GenerateTask2(int wordId)
        {
            var word = await _context.Words.FirstOrDefaultAsync(x => x.Id == wordId);
            if (word == null) throw new Exception("Word does not exist");

            return new ExerciseType2DTO
            {
                Type = "Type2",
                Sentence = await GetRandomSentence(word)
            };
        }

        private async Task<ExerciseType3DTO> GenerateTask3(int wordId)
        {
            var word = await _context.Words.FirstOrDefaultAsync(x => x.Id == wordId);
            if (word == null) throw new Exception("Word does not exist");

            var meanings = new List<string>();
            var rnd = new Random();

            var info = await GetInfoFromDictionaryAPI(word);
            if (info == null) throw new Exception("Meaning wasn't found");
            var correctMeaning = info.Meanings[0].Definitions[0].Definition;
            meanings.Add(correctMeaning);

            var size = _context.Words.Count();
            var words = _context.Words.ToList();
            for (int i = 0; i < 3; i++)
            {
                var anotherWord = words[rnd.Next(size)];
                var anotherWordInfo = await GetInfoFromDictionaryAPI(anotherWord);
                if (anotherWordInfo == null) i -= 1;
                else meanings.Add(anotherWordInfo.Meanings[0].Definitions[0].Definition);
            }

            meanings = meanings.OrderBy(a => rnd.Next()).ToList();
            return new ExerciseType3DTO
            {
                Type = "Type3",
                Meanings = meanings,
                CorrectAnswer = meanings.IndexOf(correctMeaning)
            };
        }

        private async Task<string> GetRandomSentence(Word word)
        {
            var reversoWordInfo = await GetWordDTO(word);
            var rnd = new Random();
            int randIndex = rnd.Next(reversoWordInfo.Examples.Count);
            return reversoWordInfo.Examples[randIndex].Value.Replace("<em>", "").Replace("</em>", "");
        }

        private async Task<DictionaryApiWordDTO?> GetInfoFromDictionaryAPI(Word word)
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word.Value}");
            response.EnsureSuccessStatusCode();

            var responseData = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            return GetResponse(responseData);
        }

        public DictionaryApiWordDTO? GetResponse(byte[] responseData)
        {
            var reader = new Utf8JsonReader(responseData);
            var word = JsonSerializer.Deserialize<List<DictionaryApiWordDTO>>(ref reader, new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
            })?[0];
            return word;
        }
    }
}
