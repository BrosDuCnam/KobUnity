using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utils
{
	[Serializable]
	public class SerializedDictionary<K, V> : IDictionary<K, V>, ISerializationCallbackReceiver
	{
		[Serializable]
		public struct KeyValuePair
		{
			public K key;
			public V value;
		}

		[NonSerialized] private Dictionary<K, V> _dictionary = new();
		
		[SerializeField] private List<KeyValuePair> values = new();
		
		public void OnBeforeSerialize()
		{
			if (!Application.isPlaying) return;
			
			values.Clear();
			foreach (var pair in _dictionary)
			{
				values.Add(new KeyValuePair { key = pair.Key, value = pair.Value });
			}
		}

		public void OnAfterDeserialize()
		{
			_dictionary.Clear();

			foreach (var pair in values)
			{
				_dictionary.TryAdd(pair.key, pair.value);
			}		
		}
		
		public bool IsReadOnly => false;
		public ICollection<K> Keys => _dictionary.Keys;
		public ICollection<V> Values => _dictionary.Values;

		public void Add(K key, V value)
		{
			_dictionary.Add(key, value);
		}
		
		public void Add(KeyValuePair<K, V> item)
		{
			_dictionary.Add(item.Key, item.Value);
		}

		public bool Remove(K key)
		{
			return _dictionary.Remove(key);
		}
		
		public bool Remove(KeyValuePair<K, V> item)
		{
			return _dictionary.Remove(item.Key);
		}
		
		public bool Contains(KeyValuePair<K, V> item)
		{
			return _dictionary.ContainsKey(item.Key);
		}
		
		public bool ContainsKey(K key)
		{
			return _dictionary.ContainsKey(key);
		}
		
		public bool TryGetValue(K key, out V value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			foreach (var item in _dictionary)
			{
				array[arrayIndex++] = item;
			}
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		
		public V this[K key]
		{
			get => _dictionary[key];
			set => _dictionary[key] = value;
		}
		public int Count => _dictionary.Count;

		public void Clear()
		{
			_dictionary.Clear();
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Dictionary<K, V> GetInnerDictionary()
		{
			return _dictionary;
		}
	}
}