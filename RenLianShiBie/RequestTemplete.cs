using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenLianShiBie
{
    public class wifiMsg
    {
        public string ssId;
        public string pwd;
        public Boolean isDHCPMod;
        public string ip;
        public string gateway;
        public string dns;
    }

    public class EthernetMsg
    {
        public Boolean dHCPMod;
        public string dNS1;
        public string dNS2;
        public string gateway;
        public string ip;
        public string subnetMask;
    }

    class PathTime
    {
        public string personId;
        public string passtime;
        public PathTime()
        {
            personId = "";
            passtime = "";
        }
        public PathTime(string personID)
        {
            personId = personID;
            passtime = "";
        }

        public void AddTime(string stime , string etime)
        {
            if (passtime == "")
                passtime = stime + "," + etime;
            else
                passtime = passtime + "," + stime + "," + etime;
        }

        public bool TimeIsEmpty()
        {
            return (passtime == "");
        }
    }
    class Person
    {
        public string id;
        public string idcardNum;
        public string name;
        public Person(string id, string cardnum, string name)
        {
            this.id = id;
            idcardNum = cardnum;
            this.name = name;
        }
    }

    public class PersonInfo
    {
        public long createTime;
        public string id;
        public string idcardNum;
        public string name;
    }

    public class PersonDeleteEffective
    {
        public string effective;
        public string invalid;
        public PersonDeleteEffective()
        {
            effective = "";
            invalid = "";
        }
    }

    public class PageInfo
    {
        public int index;
        public int length;
        public int size;
        public int total;
    }

    public class FindPersonResult
    {
        public PageInfo pageInfo;
        public PersonInfo[] personInfos;
    }
    
    public class FaceRecord
    {
        public int id;
        public string path;
        public string personId;
        public string name;
        public string state;
        public long time;
        public int type;
        
    }

    public class CardRecord
    {
        public int id;
        public string name;
        public string personId;
        public string state;
        public long time;
        public int type;
    }

    public class FindRecordResult<T>
    {
        public PageInfo pageinfo;
        public T[] records;
    }



    public class FindFace
    {
        public string faceId;
        public string feature; // use for img data
        public string featureKey; // no use
        public string path; // no use
        public string personId;
    }



    public class DevParameter
    {
        public string   comModContent; 
        public    int   comModType;
        public string   companyName;
        public    int   delayTimeForCloseDoor;
        public string   displayModContent; //{name}...
        public    int   displayModType;
        public int identifyDistance;
        public int identifyScores;
        public string intro;
        public int multiplayerDetection;
        //public int enrollRank;
        public int recRank;
        public int recStrangerTimesThreshold;
        public int recStrangerType;
        public int saveIdentifyTime;
        public string slogan;
        public string ttsModContent;
        public string ttsModStrangerContent;
        public int ttsModStrangerType;
        public int ttsModType;
        public string wg; //#WG{id}#

        public DevParameter()
        {

        }
    }

#pragma warning disable CS0649
    class DeviceHeartBeat
    {
        public string deviceKey;
        public long time;
        public string ip;
        public int personCount;
        public int faceCount;
        public string version;
        
    }

    class ImgRegCallBack
    {
        public string deviceKey;
        public string personId;
        public long time;
        public string imgPath;
        public string newImgPath;
        public string faceId;
        public string ip;
        public string feature;
        public string featureKey;
    }

    class IdentityCallBack
    {
        public string personId;
        public string deviceKey;
        public string type; // face_0: 时间段内，face_1:时间段外，face_2:陌生人， card_0: face样子。。
        public string ip;
        public long time;
        public string path;
        public string feature;
    }
#pragma warning restore CS0649

    class MachineInfo
    {
        public int id;
        public string name;
        public string ip;
        public int port;
        public string pwd;
    }

    public class BroadCastMsg
    {
        public string ident; // Hysoon
        public string ip;
        public string msg;
    }

    public class _ResultInfo<T>
    {
        public int result;
        public Boolean success;
        public T data;
        public String msg;

  
        public Boolean IsSucceed()
        {
            return (result == 1) && success;
        }
    }

}
