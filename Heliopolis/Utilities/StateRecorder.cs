using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Heliopolis.Utilities
{
    /// <summary>
    /// Represents a single state change in the game.
    /// </summary>
    public class StateRecord
    {
        /// <summary>
        /// The ID of the object changing state.
        /// </summary>
        public Guid Id;
        /// <summary>
        /// The name of the property changing state.
        /// </summary>
        public string Property;
        /// <summary>
        /// The type of value changing state.
        /// </summary>
        public Type ValueType;
        /// <summary>
        /// The old value
        /// </summary>
        public object OldValue;
        /// <summary>
        /// The new value
        /// </summary>
        public object NewValue;

        /// <summary>
        /// Initialises new instance of the StateRecord class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="property"></param>
        /// <param name="valueType"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public StateRecord(Guid id, string property, Type valueType, object oldValue, object newValue)
        {
            Id = id;
            Property = property;
            ValueType = valueType;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    /// <summary>
    /// This class records state changes and can write to XML.
    /// </summary>
    public class StateRecorder
    {
        static StateRecorder()
        {
            _instance = new StateRecorder();
        }

        private static readonly StateRecorder _instance;

        public static StateRecorder Instance
        {
            get { return _instance; }
        }

        private SortedDictionary<TimeSpan, StateRecord> _stateList = new SortedDictionary<TimeSpan, StateRecord>();
        private bool _recording;
        private Guid _recordingId;

        public bool Recording
        {
            get { return _recording; }
            set { _recording = value; }
        }

        private StateRecorder()
        {

        }

        public void StartRecording()
        {
            _recordingId = new Guid();
            //string filename = _recordingId.ToString() + "GameWorld.bin";
            //Serialization.SaveWorldToDiskBinary(GameWorld.Instance, filename);
            _recording = true;
        }

        public void FinishRecording()
        {
            string filename = _recordingId.ToString() + "Recording.bin";
            WriteToFile(filename);
        }

        public void AddStateChange(Guid Id, string property, Type valueType, object oldValue, object newValue, TimeSpan gameTime)
        {
            if (!_recording) return;
            StateRecord stateRecord = new StateRecord(Id, property, valueType, oldValue, newValue);
            _stateList.Add(gameTime, stateRecord);
        }

        public void WriteToFile(string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename,
                         FileMode.Create,
                         FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, _stateList);
            stream.Close();
        }

        public void LoadFromFile(string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
            _stateList = (SortedDictionary<TimeSpan, StateRecord>)formatter.Deserialize(stream);
            stream.Close();
        }    
    }
}
