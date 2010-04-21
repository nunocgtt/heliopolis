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
        public string property;
        /// <summary>
        /// The type of value changing state.
        /// </summary>
        public Type valueType;
        /// <summary>
        /// The old value
        /// </summary>
        public object oldValue;
        /// <summary>
        /// The new value
        /// </summary>
        public object newValue;

        /// <summary>
        /// Initialises new instance of the StateRecord class.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_property"></param>
        /// <param name="_valueType"></param>
        /// <param name="_oldValue"></param>
        /// <param name="_newValue"></param>
        public StateRecord(Guid _id, string _property, Type _valueType, object _oldValue, object _newValue)
        {
            Id = _id;
            property = _property;
            valueType = _valueType;
            oldValue = _oldValue;
            newValue = _newValue;
        }
    }

    /// <summary>
    /// This class records state changes and can write to XML.
    /// </summary>
    public class StateRecorder
    {
        static StateRecorder()
        {
            instance = new StateRecorder();
        }

        private static StateRecorder instance;

        public static StateRecorder Instance
        {
            get { return instance; }
        }

        private SortedDictionary<TimeSpan, StateRecord> stateList = new SortedDictionary<TimeSpan, StateRecord>();
        private bool recording;
        private Guid recordingId;

        public bool Recording
        {
            get { return recording; }
            set { recording = value; }
        }

        private StateRecorder()
        {

        }

        public void StartRecording()
        {
            recordingId = new Guid();
            string filename = recordingId.ToString() + "GameWorld.bin";
            //Serialization.SaveWorldToDiskBinary(GameWorld.Instance, filename);
            recording = true;
        }

        public void FinishRecording()
        {
            string filename = recordingId.ToString() + "Recording.bin";
            WriteToFile(filename);
        }

        public void AddStateChange(Guid Id, string property, Type valueType, object oldValue, object newValue, TimeSpan gameTime)
        {
            if (recording)
            {
                StateRecord stateRecord = new StateRecord(Id, property, valueType, oldValue, newValue);
                stateList.Add(gameTime, stateRecord);
            }
        }

        public void WriteToFile(string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename,
                         FileMode.Create,
                         FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, stateList);
            stream.Close();
        }

        public void LoadFromFile(string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
            stateList = (SortedDictionary<TimeSpan, StateRecord>)formatter.Deserialize(stream);
            stream.Close();
        }    
    }
}
