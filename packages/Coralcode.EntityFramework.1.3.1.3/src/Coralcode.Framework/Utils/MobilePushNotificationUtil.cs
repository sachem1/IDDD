using System;
using System.Collections.Generic;
using cn.jpush.api;
using cn.jpush.api.push.mode;
using cn.jpush.api.push.notification;
using cn.jpush.api.report;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;

namespace Coralcode.Framework.Utils
{
    /// <summary>
    /// 手机推送
    /// </summary>
    public class MobilePushNotificationUtil
    {
        private static string app_key = AppConfig.DllConfigs.Current["Push"]["AppKey"];
        private static string master_secret = AppConfig.DllConfigs.Current["Push"]["MasterSecret"];
        private static bool isRelease = CoralConvert.Convert<bool>(AppConfig.DllConfigs.Current["Push"]["IsRelease"] ?? "true");

        private static readonly JPushClient _client = new JPushClient(app_key, master_secret);

        /// <summary>
        /// 全平台发送通知
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static long SendAllPush(string notificationTitle, string content,
            Dictionary<string, string> keyValueParams)
        {
            return SendPush(notificationTitle, content, string.Empty, keyValueParams);
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="content">消息内容</param>
        /// <param name="pushId">用户标识</param>
        /// <returns>推送消息ID</returns>
        public static long SendPush(string notificationTitle, string content, string pushId, Dictionary<string, string> keyValueParams)
        {
            return SendPush(notificationTitle, content, DeviceType.Android & DeviceType.IOS & DeviceType.WinPhone, pushId, keyValueParams);
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="notificationTitle"></param>
        /// <param name="content">通知内容</param>
        /// <param name="message">消息内容</param>
        /// <param name="deviceTypes">推送平台</param>
        /// <param name="pushId">用户标识</param>
        /// <returns>推送消息ID</returns>
        public static long SendPush(string notificationTitle, string content, DeviceType deviceTypes, string pushId,
            Dictionary<string, string> keyValueParams)
        {
            return SendPush(notificationTitle, content, null, deviceTypes, pushId, keyValueParams);
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="notificationTitle">通知title</param>
        /// <param name="content">通知内容</param>
        /// <param name="message">消息内容</param>
        /// <param name="deviceTypes">推送平台</param>
        /// <param name="pushId">用户标识</param>
        /// <param name="keyValueParams">参数</param>
        /// <returns>推送消息ID</returns>
        public static long SendPush(
            string notificationTitle,
            string content,
            string message,
            DeviceType deviceTypes,
            string pushId,
            Dictionary<string, string> keyValueParams)
        {


            PushPayload pushPayload = new PushPayload();

            var platformNotification = GetPlatformNotification(deviceTypes, notificationTitle, content, keyValueParams);
            pushPayload.platform = platformNotification.Item1;
            pushPayload.notification = platformNotification.Item2;

            Audience audience = string.IsNullOrWhiteSpace(pushId)
                ?
                Audience.all()
                : Audience.s_registrationId(pushId);

            pushPayload.audience = audience;

            if (!string.IsNullOrWhiteSpace(message))
                pushPayload.message = Message.content(message);

            //true 表示推送生产环境， false表示要推送开发环境；
            pushPayload.ResetOptionsApnsProduction(isRelease);

            var result = _client.SendPush(pushPayload);
            return result.msg_id;
        }


        public ReceivedResult GetReceived(string msgIds)
        {
            return _client.getReceivedApi(msgIds);
        }

        public ReceivedResult GetReceivedV3(string msgIds)
        {
            return _client.getReceivedApi_v3(msgIds);
        }

        private static Tuple<Platform, Notification> GetPlatformNotification(
            DeviceType deviceTypes,
            string notificationTitle,
            string content,
            Dictionary<string, string> keyValueParams)
        {
            Platform platform;
            Notification notification = new Notification();

            if (deviceTypes.HasFlag(DeviceType.Android))
            {
                platform = Platform.android();
                notification.setAndroid(GetAndroidNotification(notificationTitle, content, keyValueParams));
            }
            else if (deviceTypes.HasFlag(DeviceType.IOS))
            {
                platform = Platform.ios();
                notification.setIos(GetIosNotification(content, keyValueParams));
            }
            else if (deviceTypes.HasFlag(DeviceType.WinPhone))
            {
                platform = Platform.winphone();
                notification.setWinphone(GetWinphoneNotification(content, keyValueParams));
            }
            else
            {
                platform = Platform.all();
                notification.setAndroid(GetAndroidNotification(notificationTitle, content, keyValueParams));
                notification.setIos(GetIosNotification(content, keyValueParams));
                notification.setWinphone(GetWinphoneNotification(content, keyValueParams));
            }


            return new Tuple<Platform, Notification>(platform, notification);
        }


        private static AndroidNotification GetAndroidNotification(
            string notificationTitle,
            string content,
            Dictionary<string, string> keyValueParams)
        {
            var androidNotification = new AndroidNotification();
            androidNotification.setTitle(notificationTitle);
            androidNotification.setAlert(content);
            foreach (var keyValueParam in keyValueParams)
            {
                androidNotification.AddExtra(keyValueParam.Key, keyValueParam.Value);
            }
            return androidNotification;
        }


        private static IosNotification GetIosNotification(
            string content,
            Dictionary<string, string> keyValueParams)
        {
            var iosNotification = new IosNotification();
            iosNotification.setAlert(content);
            foreach (var keyValueParam in keyValueParams)
            {
                iosNotification.AddExtra(keyValueParam.Key, keyValueParam.Value);
            }
            return iosNotification;
        }

        private static WinphoneNotification GetWinphoneNotification(
            string content,
            Dictionary<string, string> keyValueParams)
        {
            var winphoneNotification = new WinphoneNotification();
            winphoneNotification.setAlert(content);
            foreach (var keyValueParam in keyValueParams)
            {
                winphoneNotification.AddExtra(keyValueParam.Key, keyValueParam.Value);
            }
            return winphoneNotification;
        }

    }
}
