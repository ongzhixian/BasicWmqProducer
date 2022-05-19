
using IBM.XMS;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Reflection;

const string APP_SETTINGS_CONFIGURATION_FILE = "appsettings.json";

var logger = LogManager.GetCurrentClassLogger();

// CONFIGURATION_KEYs

const string WMQ_HOST_NAME_CONFIGURATION_KEY = "IBM_MQ:WMQ_HOST_NAME";
const string WMQ_PORT_CONFIGURATION_KEY = "IBM_MQ:WMQ_PORT";
const string WMQ_CHANNEL_CONFIGURATION_KEY = "IBM_MQ:WMQ_CHANNEL";
const string WMQ_QUEUE_MANAGER_CONFIGURATION_KEY = "IBM_MQ:WMQ_QUEUE_MANAGER";
const string WMQ_MESSAGE_DESTINATION_CONFIGURATION_KEY = "IBM_MQ:WMQ_MESSAGE_DESTINATION";
const string WMQ_APPLICATIONNAME_CONFIGURATION_KEY = "IBM_MQ:WMQ_APPLICATIONNAMEa";

const string WMQ_USERID_CONFIGURATION_KEY = "ibm_mq:userId";
const string WMQ_PASSWORD_CONFIGURATION_KEY = "ibm_mq:password";

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile(APP_SETTINGS_CONFIGURATION_FILE, optional: false, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
    .Build();

IConnectionFactory connectionFactory = GetConnectionFactory(configuration);

string wmqMessageDestination = configuration[WMQ_MESSAGE_DESTINATION_CONFIGURATION_KEY];

using (IConnection connection = connectionFactory.CreateConnection())
using (ISession sessionWMQ = connection.CreateSession(false, AcknowledgeMode.AutoAcknowledge))
using (IDestination destination = sessionWMQ.CreateQueue(wmqMessageDestination))
using (IMessageProducer? producer = sessionWMQ.CreateProducer(destination))
{
    connection.Start();

    while (true)
    {
        ITextMessage? textMessage = sessionWMQ.CreateTextMessage();
        textMessage.Text = "This is a simple message from XMS.NET producer";
        producer.Send(textMessage);

        logger.Info("Message sent.");

        Thread.Sleep(3000);
    }
}

IConnectionFactory GetConnectionFactory(IConfiguration configuration)
{
    string wmqApplicationName = configuration[WMQ_APPLICATIONNAME_CONFIGURATION_KEY];
    string wmqHostName = configuration[WMQ_HOST_NAME_CONFIGURATION_KEY];
    int wmqPort = int.Parse(configuration[WMQ_PORT_CONFIGURATION_KEY]);
    string wmqChannel = configuration[WMQ_CHANNEL_CONFIGURATION_KEY];
    string wmsQueueManager = configuration[WMQ_QUEUE_MANAGER_CONFIGURATION_KEY];
    string wmqUserId = configuration[WMQ_USERID_CONFIGURATION_KEY];
    string wmqPassword = configuration[WMQ_PASSWORD_CONFIGURATION_KEY];

    XMSFactoryFactory? factoryFactory = XMSFactoryFactory.GetInstance(XMSC.CT_WMQ);
    var connectionFactory = factoryFactory.CreateConnectionFactory();

    // Set the properties
    connectionFactory.SetIntProperty(XMSC.WMQ_CONNECTION_MODE, XMSC.WMQ_CM_CLIENT);

    connectionFactory.SetStringProperty(XMSC.WMQ_HOST_NAME, wmqHostName);
    connectionFactory.SetIntProperty(XMSC.WMQ_PORT, wmqPort);
    connectionFactory.SetStringProperty(XMSC.WMQ_CHANNEL, wmqChannel);
    connectionFactory.SetStringProperty(XMSC.WMQ_QUEUE_MANAGER, wmsQueueManager);

    connectionFactory.SetStringProperty(XMSC.USERID, wmqUserId);
    connectionFactory.SetStringProperty(XMSC.PASSWORD, wmqPassword);

    // Set Application ID / Name; defaults to executable "BasicWmqProducer.ConsoleApp" if not set)
    if (!string.IsNullOrWhiteSpace(wmqApplicationName))
    {
        connectionFactory.SetStringProperty(XMSC.WMQ_APPLICATIONNAME, wmqApplicationName);
    }

    // SSL properties (TODO)
    //if ((String)properties[XMSC.WMQ_SSL_KEY_REPOSITORY] != "") cf.SetStringProperty(XMSC.WMQ_SSL_KEY_REPOSITORY, (String)properties[XMSC.WMQ_SSL_KEY_REPOSITORY]);
    //if ((String)properties[XMSC.WMQ_SSL_CIPHER_SPEC] != "") cf.SetStringProperty(XMSC.WMQ_SSL_CIPHER_SPEC, (String)properties[XMSC.WMQ_SSL_CIPHER_SPEC]);
    //if ((String)properties[XMSC.WMQ_SSL_PEER_NAME] != "") cf.SetStringProperty(XMSC.WMQ_SSL_PEER_NAME, (String)properties[XMSC.WMQ_SSL_PEER_NAME]);
    //if ((Int32)properties[XMSC.WMQ_SSL_KEY_RESETCOUNT] != -1) cf.SetIntProperty(XMSC.WMQ_SSL_KEY_RESETCOUNT, (Int32)properties[XMSC.WMQ_SSL_KEY_RESETCOUNT]);
    //if ((Boolean)properties[XMSC.WMQ_SSL_CERT_REVOCATION_CHECK] != false) cf.SetBooleanProperty(XMSC.WMQ_SSL_CERT_REVOCATION_CHECK, true);


    return connectionFactory;
}